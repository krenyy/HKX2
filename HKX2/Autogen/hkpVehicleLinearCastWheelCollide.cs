using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleLinearCastWheelCollide : hkpVehicleWheelCollide
    {
        public override uint Signature { get => 0; }
        
        public uint m_wheelCollisionFilterInfo;
        public List<hkpVehicleLinearCastWheelCollideWheelState> m_wheelStates;
        public hkpRejectChassisListener m_rejectChassisListener;
        public float m_maxExtraPenetration;
        public float m_startPointTolerance;
        public bool m_collectStartPointHits;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelCollisionFilterInfo = br.ReadUInt32();
            br.ReadUInt32();
            m_wheelStates = des.ReadClassArray<hkpVehicleLinearCastWheelCollideWheelState>(br);
            m_rejectChassisListener = new hkpRejectChassisListener();
            m_rejectChassisListener.Read(des, br);
            m_maxExtraPenetration = br.ReadSingle();
            m_startPointTolerance = br.ReadSingle();
            m_collectStartPointHits = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt32(m_wheelCollisionFilterInfo);
            bw.WriteUInt32(0);
            s.WriteClassArray<hkpVehicleLinearCastWheelCollideWheelState>(bw, m_wheelStates);
            m_rejectChassisListener.Write(s, bw);
            bw.WriteSingle(m_maxExtraPenetration);
            bw.WriteSingle(m_startPointTolerance);
            bw.WriteBoolean(m_collectStartPointHits);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
