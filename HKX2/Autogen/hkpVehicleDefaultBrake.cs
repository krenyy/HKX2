using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultBrake : hkpVehicleBrake
    {
        public override uint Signature { get => 0; }
        
        public List<hkpVehicleDefaultBrakeWheelBrakingProperties> m_wheelBrakingProperties;
        public float m_wheelsMinTimeToBlock;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelBrakingProperties = des.ReadClassArray<hkpVehicleDefaultBrakeWheelBrakingProperties>(br);
            m_wheelsMinTimeToBlock = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray<hkpVehicleDefaultBrakeWheelBrakingProperties>(bw, m_wheelBrakingProperties);
            bw.WriteSingle(m_wheelsMinTimeToBlock);
            bw.WriteUInt32(0);
        }
    }
}
