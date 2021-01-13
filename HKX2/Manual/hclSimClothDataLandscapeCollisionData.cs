namespace HKX2
{
    public partial class hclSimClothDataLandscapeCollisionData : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public float m_landscapeRadius;
        public bool m_enableStuckParticleDetection;
        public float m_stuckParticlesStretchFactorSq;
        public bool m_pinchDetectionEnabled;
        public sbyte m_pinchDetectionPriority;
        public float m_pinchDetectionRadius;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_landscapeRadius = br.ReadSingle();
            m_enableStuckParticleDetection = br.ReadBoolean();
            br.ReadByte();
            br.ReadUInt16();
            m_stuckParticlesStretchFactorSq = br.ReadSingle();
            m_pinchDetectionEnabled = br.ReadBoolean();
            m_pinchDetectionPriority = br.ReadSByte();
            br.ReadUInt16();
            m_pinchDetectionRadius = br.ReadSingle();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_landscapeRadius);
            bw.WriteBoolean(m_enableStuckParticleDetection);
            bw.WriteByte(0);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_stuckParticlesStretchFactorSq);
            bw.WriteBoolean(m_pinchDetectionEnabled);
            bw.WriteSByte(m_pinchDetectionPriority);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_pinchDetectionRadius);
        }
    }
}
