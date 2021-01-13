using System.Numerics;

namespace HKX2
{
    public partial class hkSphere : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public Vector4 m_pos;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.Pad(16);
            m_pos = des.ReadVector4(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.Pad(16);
            s.WriteVector4(bw, m_pos);
        }
    }
}
