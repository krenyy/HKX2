using System.Numerics;

namespace HKX2
{
    public partial class hclVolumeConstraintApplyData : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public Vector4 m_frameVector;
        public ushort m_particleIndex;
        public float m_stiffness;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.Pad(16);
            m_frameVector = des.ReadVector4(br);
            m_particleIndex = br.ReadUInt16();
            br.ReadUInt16();
            m_stiffness = br.ReadSingle();

            if (des._header.PointerSize == 8)
            {
                br.ReadUInt64();
            }
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.Pad(16);
            s.WriteVector4(bw, m_frameVector);
            bw.WriteUInt16(m_particleIndex);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_stiffness);

            if (s._header.PointerSize == 8)
            {
                bw.WriteUInt64(0);
            }
        }
    }
}
