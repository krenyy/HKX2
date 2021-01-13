using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiAvoidancePairPropertiesPairData : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public uint m_key;
        public float m_weight;
        public float m_cosViewAngle;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_key = br.ReadUInt32();
            m_weight = br.ReadSingle();
            m_cosViewAngle = br.ReadSingle();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_key);
            bw.WriteSingle(m_weight);
            bw.WriteSingle(m_cosViewAngle);
        }
    }
}
