using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleManager : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public List<hkpVehicleInstance> m_registeredVehicles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_registeredVehicles = des.ReadClassPointerArray<hkpVehicleInstance>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray<hkpVehicleInstance>(bw, m_registeredVehicles);
        }
    }
}
