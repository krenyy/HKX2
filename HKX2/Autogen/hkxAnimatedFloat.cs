using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkxAnimatedFloat : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public List<float> m_floats;
        public Hint m_hint;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_floats = des.ReadSingleArray(br);
            m_hint = (Hint)br.ReadByte();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_floats);
            bw.WriteByte((byte)m_hint);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
