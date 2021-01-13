using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpLinMotorConstraintAtom : hkpConstraintAtom
    {
        public override uint Signature { get => 0; }
        
        public bool m_isEnabled;
        public byte m_motorAxis;
        public hkpConstraintMotor m_motor;
        public float m_targetPosition;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadBoolean();
            m_motorAxis = br.ReadByte();
            br.ReadUInt32();
            m_motor = des.ReadClassPointer<hkpConstraintMotor>(br);
            m_targetPosition = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteByte(m_motorAxis);
            bw.WriteUInt32(0);
            s.WriteClassPointer<hkpConstraintMotor>(bw, m_motor);
            bw.WriteSingle(m_targetPosition);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
