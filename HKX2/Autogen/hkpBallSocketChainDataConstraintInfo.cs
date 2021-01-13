using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpBallSocketChainDataConstraintInfo : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public Vector4 m_pivotInA;
        public Vector4 m_pivotInB;
        public uint m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivotInA = des.ReadVector4(br);
            m_pivotInB = des.ReadVector4(br);
            m_flags = br.ReadUInt32();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVector4(bw, m_pivotInA);
            s.WriteVector4(bw, m_pivotInB);
            bw.WriteUInt32(m_flags);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
