using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultSimulation : hkpVehicleSimulation
    {
        public override uint Signature { get => 0; }
        
        public hkpVehicleFrictionStatus m_frictionStatus;
        public hkpVehicleFrictionDescription m_frictionDescription;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_frictionStatus = new hkpVehicleFrictionStatus();
            m_frictionStatus.Read(des, br);
            br.ReadUInt32();
            m_frictionDescription = des.ReadClassPointer<hkpVehicleFrictionDescription>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_frictionStatus.Write(s, bw);
            bw.WriteUInt32(0);
            s.WriteClassPointer<hkpVehicleFrictionDescription>(bw, m_frictionDescription);
        }
    }
}
