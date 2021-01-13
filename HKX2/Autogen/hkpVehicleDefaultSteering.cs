using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultSteering : hkpVehicleSteering
    {
        public override uint Signature { get => 0; }
        
        public float m_maxSteeringAngle;
        public float m_maxSpeedFullSteeringAngle;
        public List<bool> m_doesWheelSteer;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_maxSteeringAngle = br.ReadSingle();
            m_maxSpeedFullSteeringAngle = br.ReadSingle();
            br.ReadUInt32();
            m_doesWheelSteer = des.ReadBooleanArray(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_maxSteeringAngle);
            bw.WriteSingle(m_maxSpeedFullSteeringAngle);
            bw.WriteUInt32(0);
            s.WriteBooleanArray(bw, m_doesWheelSteer);
        }
    }
}
