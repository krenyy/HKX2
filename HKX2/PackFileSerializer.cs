﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace HKX2
{
    public class PackFileSerializer
    {
        public HKXHeader _header;
        
        private List<LocalFixup> _localFixups = new List<LocalFixup>();
        private List<GlobalFixup> _globalFixups = new List<GlobalFixup>();
        private List<VirtualFixup> _virtualFixups = new List<VirtualFixup>();
        
        private Dictionary<IHavokObject, uint> _globalLookup = new Dictionary<IHavokObject, uint>();
        private Dictionary<string, uint> _virtualLookup = new Dictionary<string, uint>();

        private int _currentLocalWriteQueue;
        private List<Queue<Action>> _localWriteQueues = new List<Queue<Action>>();
        private int _currentSerializationQueue;
        private List<Queue<IHavokObject>> _serializationQueues = new List<Queue<IHavokObject>>();

        private HashSet<IHavokObject> _pendingVirtuals = new HashSet<IHavokObject>();
        private Dictionary<IHavokObject, List<uint>> _pendingGlobals = new Dictionary<IHavokObject, List<uint>>();

        private HashSet<IHavokObject> _serializedObjects = new HashSet<IHavokObject>();

        #region Write methods

        public void PadToPointerSizeIfPaddingOption(BinaryWriterEx bw)
        {
            if (this._header.PaddingOption == 1)
            {
                bw.Pad(this._header.PointerSize);
            }
        }

        public void WriteEmptyPointer(BinaryWriterEx bw)
        {
            PadToPointerSizeIfPaddingOption(bw);
            bw.WriteUSize(0);
        }

        public void WriteEmptyArray(BinaryWriterEx bw)
        {
            WriteEmptyPointer(bw);
            bw.WriteUInt32(0);
            bw.WriteUInt32(0 | ((uint) 0x80 << 24));
        }
        
        private void WriteArrayBase<T>(BinaryWriterEx bw, IList<T> l, Action<T> perElement, bool pad=false)
        {
            PadToPointerSizeIfPaddingOption(bw);
            
            var src = (uint) bw.Position;
            uint size = l != null ? (uint) l.Count : 0;
            
            bw.WriteUSize(0);
            bw.WriteUInt32(size);
            bw.WriteUInt32(size | ((uint)0x80 << 24));
            
            if (size <= 0) return;
            
            LocalFixup lfu = new LocalFixup();
            lfu.Src = src;
            _localFixups.Add(lfu);
            _localWriteQueues[_currentLocalWriteQueue].Enqueue(() =>
            {
                bw.Pad(16);
                lfu.Dst = (uint)bw.Position;
                // This ensures any writes the array elements may have are top priority
                PushLocalWriteQueue();
                for (int i = 0; i < size; i++)
                {
                    perElement.Invoke(l[i]);
                }
                PopLocalWriteQueue(bw);
            });
            if (pad)
            {
                _localWriteQueues[_currentLocalWriteQueue].Enqueue(() =>
                {
                    bw.Pad(16);
                });
            }
        }

        public void WriteClassArray<T>(BinaryWriterEx bw, List<T> d) where T : IHavokObject
        {
            WriteArrayBase(bw, d, e =>
            {
                e.Write(this, bw);
            }, true);
        }

        public void WriteClassPointer<T>(BinaryWriterEx bw, T d) where T : IHavokObject
        {
            PadToPointerSizeIfPaddingOption(bw);
            var pos = (uint) bw.Position;
            bw.WriteUSize(0);
            
            if (d == null)
            {
                return;
            }

            // If we're referencing an already serialized object, add a global ref
            if (_globalLookup.ContainsKey(d))
            {
                GlobalFixup gfu = new GlobalFixup();
                gfu.Src = pos;
                gfu.DstSectionIndex = 2;
                gfu.Dst = _globalLookup[d];
                _globalFixups.Add(gfu);
                return;
            }
            // Otherwise need to add a pending reference and mark the object for serialization

            if (!_pendingGlobals.ContainsKey(d))
            {
                _pendingGlobals.Add(d, new List<uint>());
                PushSerializationQueue();
                _serializationQueues[_currentSerializationQueue].Enqueue(d);
                PopSerializationQueue(bw);
                _pendingVirtuals.Add(d);
            }
            _pendingGlobals[d].Add(pos);
        }

        public void WriteClassPointerArray<T>(BinaryWriterEx bw, List<T> d) where T : IHavokObject
        {
            WriteArrayBase(bw, d, e => WriteClassPointer(bw, e));
        }

        public void WriteStringPointer(BinaryWriterEx bw, string d, int padding = 16)
        {
            PadToPointerSizeIfPaddingOption(bw);
            var src = (uint)bw.Position;
            bw.WriteUSize(0);
            
            if (d == null) return;
            
            var lfu = new LocalFixup();
            lfu.Src = src;
            _localFixups.Add(lfu);
            _localWriteQueues[_currentLocalWriteQueue].Enqueue(() =>
            {
                lfu.Dst = (uint) bw.Position;
                bw.WriteASCII(d, true);
                bw.Pad(padding);
            });
        }

        public void WriteStringPointerArray(BinaryWriterEx bw, List<string> d)
        {
            WriteArrayBase(bw, d, (e) =>
            {
                WriteStringPointer(bw, e, 2);
            });
        }

        public void WriteByte(BinaryWriterEx bw, byte d)
        {
            bw.WriteByte(d);
        }
        
        public void WriteByteArray(BinaryWriterEx bw, List<byte> d)
        {
            WriteArrayBase(bw, d, (e) => WriteByte(bw, e));
        }

        public void WriteSByte(BinaryWriterEx bw, sbyte d)
        {
            bw.WriteSByte(d);
        }
        
        public void WriteSByteArray(BinaryWriterEx bw, List<sbyte> d)
        {
            WriteArrayBase(bw, d, e => WriteSByte(bw, e));
        }

        public void WriteUInt16(BinaryWriterEx bw, ushort d)
        {
            bw.WriteUInt16(d);
        }

        public void WriteUInt16Array(BinaryWriterEx bw, List<ushort> d)
        {
            WriteArrayBase(bw, d, (e) => WriteUInt16(bw, e));
        }

        public void WriteInt16(BinaryWriterEx bw, short d)
        {
            bw.WriteInt16(d);
        }
        
        public void WriteInt16Array(BinaryWriterEx bw, List<short> d)
        {
            WriteArrayBase(bw, d, e => WriteInt16(bw, e));
        }

        public void WriteUInt32(BinaryWriterEx bw, uint d)
        {
            bw.WriteUInt32(d);
        }
        
        public void WriteUInt32Array(BinaryWriterEx bw, List<uint> d)
        {
            WriteArrayBase(bw, d, e => WriteUInt32(bw, e));
        }

        public void WriteInt32(BinaryWriterEx bw, int d)
        {
            bw.WriteInt32(d);
        }
        
        public void WriteInt32Array(BinaryWriterEx bw, List<int> d)
        {
            WriteArrayBase(bw, d, e => WriteInt32(bw, e));
        }

        public void WriteUInt64(BinaryWriterEx bw, ulong d)
        {
            bw.WriteUInt64(d);
        }
        
        public void WriteUInt64Array(BinaryWriterEx bw, List<ulong> d)
        {
            WriteArrayBase(bw, d, (e) => WriteUInt64(bw, e));
        }

        public void WriteInt64(BinaryWriterEx bw, long d)
        {
            bw.WriteInt64(d);
        }
        
        public void WriteInt64Array(BinaryWriterEx bw, List<long> d)
        {
            WriteArrayBase(bw, d, e => WriteInt64(bw, e));
        }

        public void WriteSingle(BinaryWriterEx bw, float d)
        {
            bw.WriteSingle(d);
        }
        
        public void WriteSingleArray(BinaryWriterEx bw, List<float> d)
        {
            WriteArrayBase(bw, d, e => WriteSingle(bw, e));
        }

        public void WriteBoolean(BinaryWriterEx bw, bool d)
        {
            bw.WriteBoolean(d);
        }
        
        public void WriteBooleanArray(BinaryWriterEx bw, List<bool> d)
        {
            WriteArrayBase(bw, d, e => WriteBoolean(bw, e));
        }

        public void WriteVector4(BinaryWriterEx bw, Vector4 d)
        {
            bw.WriteVector4(d);
        }

        public void WriteVector4Array(BinaryWriterEx bw, List<Vector4> d)
        {
            WriteArrayBase(bw, d, e => WriteVector4(bw, e));
        }

        public void WriteMatrix3(BinaryWriterEx bw, Matrix4x4 d)
        {
            bw.WriteSingle(d.M11);
            bw.WriteSingle(d.M12);
            bw.WriteSingle(d.M13);
            bw.WriteSingle(d.M14);
            bw.WriteSingle(d.M21);
            bw.WriteSingle(d.M22);
            bw.WriteSingle(d.M23);
            bw.WriteSingle(d.M24);
            bw.WriteSingle(d.M31);
            bw.WriteSingle(d.M32);
            bw.WriteSingle(d.M33);
            bw.WriteSingle(d.M34);
        }

        public void WriteMatrix3Array(BinaryWriterEx bw, List<Matrix4x4> d)
        {
            WriteArrayBase(bw, d, e => WriteMatrix3(bw, e));
        }

        public void WriteMatrix4(BinaryWriterEx bw, Matrix4x4 d)
        {
            bw.WriteSingle(d.M11);
            bw.WriteSingle(d.M12);
            bw.WriteSingle(d.M13);
            bw.WriteSingle(d.M14);
            bw.WriteSingle(d.M21);
            bw.WriteSingle(d.M22);
            bw.WriteSingle(d.M23);
            bw.WriteSingle(d.M24);
            bw.WriteSingle(d.M31);
            bw.WriteSingle(d.M32);
            bw.WriteSingle(d.M33);
            bw.WriteSingle(d.M34);
            bw.WriteSingle(d.M41);
            bw.WriteSingle(d.M42);
            bw.WriteSingle(d.M43);
            bw.WriteSingle(d.M44);
        }

        public void WriteMatrix4Array(BinaryWriterEx bw, List<Matrix4x4> d)
        {
            WriteArrayBase(bw, d, e => WriteMatrix4(bw, e));
        }

        public void WriteTransform(BinaryWriterEx bw, Matrix4x4 d)
        {
            bw.WriteSingle(d.M11);
            bw.WriteSingle(d.M12);
            bw.WriteSingle(d.M13);
            bw.WriteSingle(d.M14);
            bw.WriteSingle(d.M21);
            bw.WriteSingle(d.M22);
            bw.WriteSingle(d.M23);
            bw.WriteSingle(d.M24);
            bw.WriteSingle(d.M31);
            bw.WriteSingle(d.M32);
            bw.WriteSingle(d.M33);
            bw.WriteSingle(d.M34);
            bw.WriteSingle(d.M41);
            bw.WriteSingle(d.M42);
            bw.WriteSingle(d.M43);
            bw.WriteSingle(d.M44);
        }

        public void WriteTransformArray(BinaryWriterEx bw, List<Matrix4x4> d)
        {
            WriteArrayBase(bw, d, e => WriteTransform(bw, e));
        }

        public void WriteQSTransform(BinaryWriterEx bw, Matrix4x4 d)
        {
            WriteMatrix3(bw, d);
        }

        public void WriteQSTransformArray(BinaryWriterEx bw, List<Matrix4x4> d)
        {
            WriteMatrix3Array(bw, d);
        }

        public void WriteQuaternion(BinaryWriterEx bw, Quaternion d)
        {
            bw.WriteSingle(d.X);
            bw.WriteSingle(d.Y);
            bw.WriteSingle(d.Z);
            bw.WriteSingle(d.W);
        }

        public void WriteQuaternionArray(BinaryWriterEx bw, List<Quaternion> d)
        {
            WriteArrayBase(bw, d, e => WriteQuaternion(bw, e));
        }
        
        #endregion
        
        
        internal void PushLocalWriteQueue()
        {
            _currentLocalWriteQueue++;
            if (_currentLocalWriteQueue == _localWriteQueues.Count)
            {
                _localWriteQueues.Add(new Queue<Action>());
            }
        }

        internal void PopLocalWriteQueue(BinaryWriterEx bw)
        {
            // Enqueue a padding operation
            /*_localWriteQueues[_currentLocalWriteQueue].Enqueue(() =>
            {
                bw.Pad(16);
            });*/
            _currentLocalWriteQueue--;
        }

        internal void PushSerializationQueue()
        {
            _currentSerializationQueue++;
            if (_currentSerializationQueue == _serializationQueues.Count)
            {
                _serializationQueues.Add(new Queue<IHavokObject>());
            }
        }

        internal void PopSerializationQueue(BinaryWriterEx bw)
        {
            // Enqueue a padding operation
            /*_localWriteQueues[_currWriteQueue].Enqueue(() =>
            {
                bw.Pad(16);
            });*/
            _currentSerializationQueue--;
        }

        
        public void Serialize(IHavokObject rootObject, BinaryWriterEx bw, HKXHeader header)
        {
            _header = header;
            bw.BigEndian = _header.Endian == 0;
            
            _header.Write(bw);

            // Initialize bookkeeping structures
            _localFixups = new List<LocalFixup>();
            _globalFixups = new List<GlobalFixup>();
            _virtualFixups = new List<VirtualFixup>();
            
            _globalLookup = new Dictionary<IHavokObject, uint>();
            _virtualLookup = new Dictionary<string, uint>();
            
            _localWriteQueues = new List<Queue<Action>>();
            _serializationQueues = new List<Queue<IHavokObject>>();
            _pendingGlobals = new Dictionary<IHavokObject, List<uint>>();
            _pendingVirtuals = new HashSet<IHavokObject>();

            _serializedObjects = new HashSet<IHavokObject>();

            // Memory stream for writing all the class definitions
            MemoryStream classms = new MemoryStream();
            BinaryWriterEx classbw = new BinaryWriterEx(
                _header.Endian == 0, _header.PointerSize == 8, classms);

            // Data memory stream for havok objects
            MemoryStream datams = new MemoryStream();
            BinaryWriterEx databw = new BinaryWriterEx(
                _header.Endian == 0, _header.PointerSize == 8, datams);

            // Populate class names with some stuff havok always has
            HKXClassName hkclass = new HKXClassName();
            hkclass.ClassName = "hkClass";
            hkclass.Signature = 0x33D42383;
            hkclass.Write(classbw);
            hkclass.ClassName = "hkClassMember";
            hkclass.Signature = 0xB0EFA719;
            hkclass.Write(classbw);
            hkclass.ClassName = "hkClassEnum";
            hkclass.Signature = 0x8A3609CF;
            hkclass.Write(classbw);
            hkclass.ClassName = "hkClassEnumItem";
            hkclass.Signature = 0xCE6F8A6C;
            hkclass.Write(classbw);
            
            _serializationQueues.Add(new Queue<IHavokObject>());
            _serializationQueues[0].Enqueue(rootObject);
            _localWriteQueues.Add(new Queue<Action>());
            _pendingVirtuals.Add(rootObject);
            
            while (_serializationQueues.Count > 1 || _serializationQueues[0].Count > 0)
            {
                var sq = _serializationQueues.Last();
                
                while (sq != null && sq.Count() == 0 && _serializationQueues.Count > 1)
                {
                    _serializationQueues.RemoveAt(_serializationQueues.Count - 1);
                    sq = _serializationQueues.Last();
                }
                
                if (sq.Count == 0)
                {
                    continue;
                }
                
                var obj = sq.Dequeue();
                _currentSerializationQueue = _serializationQueues.Count - 1;

                if (_serializedObjects.Contains(obj))
                {
                    continue;
                }
                
                // See if we need to add virtual bookkeeping
                if (_pendingVirtuals.Contains(obj))
                {
                    _pendingVirtuals.Remove(obj);
                    var classname = obj.GetType().Name;
                    if (!_virtualLookup.ContainsKey(classname))
                    {
                        // Need to create a new class name entry and record the position
                        HKXClassName cname = new HKXClassName();
                        cname.ClassName = classname;
                        cname.Signature = obj.Signature;
                        uint offset = (uint)classbw.Position;
                        cname.Write(classbw);
                        _virtualLookup.Add(classname, offset + 5);
                    }
                    // Create a new Virtual fixup for this object
                    var vfu = new VirtualFixup();
                    vfu.Src = (uint)databw.Position;
                    vfu.DstSectionIndex = 0;
                    vfu.NameOffset = _virtualLookup[classname];
                    _virtualFixups.Add(vfu);

                    // See if we have any pending global references to this object
                    if (_pendingGlobals.ContainsKey(obj))
                    {
                        // If so, create all the needed global fixups
                        foreach (var src in _pendingGlobals[obj])
                        {
                            var gfu = new GlobalFixup();
                            gfu.Src = src;
                            gfu.DstSectionIndex = 2;
                            gfu.Dst = (uint) databw.Position;
                            _globalFixups.Add(gfu);
                        }
                        _pendingGlobals.Remove(obj);
                    }

                    // Add global lookup
                    _globalLookup.Add(obj, (uint)databw.Position);
                }
                obj.Write(this, databw);
                _serializedObjects.Add(obj);
                databw.Pad(16);

                // Write local data (such as array contents and strings)
                while (_localWriteQueues.Count > 1 || _localWriteQueues[0].Count > 0)
                {
                    var q = _localWriteQueues.Last();
                    while (q != null && q.Count() == 0 && _localWriteQueues.Count > 1)
                    {
                        if (_localWriteQueues.Count > 1)
                        {
                            _localWriteQueues.RemoveAt(_localWriteQueues.Count - 1);
                        }
                        q = _localWriteQueues.Last();

                        // Do alignment at the popping of a queue frame
                        databw.Pad(16);
                    }
                    if (q.Count == 0)
                    {
                        continue;
                    }
                    var act = q.Dequeue();
                    _currentLocalWriteQueue = _localWriteQueues.Count - 1;
                    act.Invoke();
                }
                databw.Pad(16);
            }

            HKXSection classNames = new HKXSection();
            classNames.SectionID = 0;
            classNames.SectionTag = "__classnames__";
            classNames.SectionData = classms.ToArray();
            classNames.WriteHeader(bw);

            HKXSection types = new HKXSection();
            types.SectionID = 1;
            types.SectionTag = "__types__";
            types.SectionData = new byte[0];
            types.WriteHeader(bw);

            HKXSection data = new HKXSection();
            data.SectionID = 2;
            data.SectionTag = "__data__";
            data.SectionData = datams.ToArray();
            data.LocalFixups = _localFixups.OrderBy(x => x.Dst).ToList();
            data.GlobalFixups = _globalFixups.OrderBy(x => x.Src).ToList();
            data.VirtualFixups = _virtualFixups;
            data.WriteHeader(bw);

            classNames.WriteData(bw);
            types.WriteData(bw);
            data.WriteData(bw);
        }
    }
}
