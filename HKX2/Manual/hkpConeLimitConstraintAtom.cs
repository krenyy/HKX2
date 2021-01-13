namespace HKX2
{
    public enum MeasurementMode
    {
        ZERO_WHEN_VECTORS_ALIGNED = 0,
        ZERO_WHEN_VECTORS_PERPENDICULAR = 1,
    }
    
    public partial class hkpConeLimitConstraintAtom : hkpConstraintAtom
    {
        public override uint Signature => 0x0;

        public byte m_isEnabled;
        public byte m_twistAxisInA;
        public byte m_refAxisInB;
        public MeasurementMode m_angleMeasurementMode;
        public byte m_memOffsetToAngleOffset;
        public float m_minAngle;
        public float m_maxAngle;
        public float m_angularLimitsTauFactor;
        public float m_angularLimitsDampFactor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_twistAxisInA = br.ReadByte();
            m_refAxisInB = br.ReadByte();
            m_angleMeasurementMode = (MeasurementMode)br.ReadByte();
            m_memOffsetToAngleOffset = br.ReadByte();
            br.ReadByte();
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
            m_angularLimitsTauFactor = br.ReadSingle();
            m_angularLimitsDampFactor = br.ReadSingle();
            br.ReadUInt64(); // padding_0,1,2,3,4,5,6,7
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_twistAxisInA);
            bw.WriteByte(m_refAxisInB);
            bw.WriteByte((byte)m_angleMeasurementMode);
            bw.WriteByte(m_memOffsetToAngleOffset);
            bw.WriteByte(0);
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
            bw.WriteSingle(m_angularLimitsTauFactor);
            bw.WriteSingle(m_angularLimitsDampFactor);
            bw.WriteUInt64(0); // padding_0,1,2,3,4,5,6,7
        }
    }
}
