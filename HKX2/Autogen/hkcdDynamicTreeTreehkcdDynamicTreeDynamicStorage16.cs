using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdDynamicTreeTreehkcdDynamicTreeDynamicStorage16 : hkcdDynamicTreeDynamicStorage16
    {
        public override uint Signature { get => 0; }
        
        public uint m_numLeaves;
        public uint m_path;
        public ushort m_root;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numLeaves = br.ReadUInt32();
            m_path = br.ReadUInt32();
            m_root = br.ReadUInt16();
            br.ReadUInt16();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt32(m_numLeaves);
            bw.WriteUInt32(m_path);
            bw.WriteUInt16(m_root);
            bw.WriteUInt16(0);
        }
    }
}
