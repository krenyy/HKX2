using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleRayCastWheelCollide : hkpVehicleWheelCollide
    {
        public override uint Signature { get => 0; }
        
        public uint m_wheelCollisionFilterInfo;
        public hkpAabbPhantom m_phantom;
        public hkpRejectChassisListener m_rejectRayChassisListener;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelCollisionFilterInfo = br.ReadUInt32();
            br.ReadUInt32();
            m_phantom = des.ReadClassPointer<hkpAabbPhantom>(br);
            m_rejectRayChassisListener = new hkpRejectChassisListener();
            m_rejectRayChassisListener.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt32(m_wheelCollisionFilterInfo);
            bw.WriteUInt32(0);
            s.WriteClassPointer<hkpAabbPhantom>(bw, m_phantom);
            m_rejectRayChassisListener.Write(s, bw);
        }
    }
}
