using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultSuspension : hkpVehicleSuspension
    {
        public override uint Signature { get => 0; }
        
        public List<hkpVehicleDefaultSuspensionWheelSpringSuspensionParameters> m_wheelSpringParams;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelSpringParams = des.ReadClassArray<hkpVehicleDefaultSuspensionWheelSpringSuspensionParameters>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray<hkpVehicleDefaultSuspensionWheelSpringSuspensionParameters>(bw, m_wheelSpringParams);
        }
    }
}
