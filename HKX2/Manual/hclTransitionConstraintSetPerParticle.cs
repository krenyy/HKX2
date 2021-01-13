namespace HKX2
{
    public partial class hclTransitionConstraintSetPerParticle : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public ushort m_particleIndex;
        public ushort m_referenceVertex;
        public float m_toAnimDelay;
        public float m_toSimDelay;
        public float m_toSimMaxDistance;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_particleIndex = br.ReadUInt16();
            m_referenceVertex = br.ReadUInt16();
            m_toAnimDelay = br.ReadSingle();
            m_toSimDelay = br.ReadSingle();
            m_toSimMaxDistance = br.ReadSingle();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_particleIndex);
            bw.WriteUInt16(m_referenceVertex);
            bw.WriteSingle(m_toAnimDelay);
            bw.WriteSingle(m_toSimDelay);
            bw.WriteSingle(m_toSimMaxDistance);
        }
    }
}
