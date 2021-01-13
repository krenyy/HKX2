using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehiclePerWheelSimulation : hkpVehicleSimulation
    {
        public override uint Signature { get => 0; }
        
        public hkpVehicleInstance m_instance;
        public float m_slipDamping;
        public float m_impulseScaling;
        public float m_maxImpulse;
        public float m_takeDynamicVelocity;
        public float m_curbDamping;
        public List<hkpVehiclePerWheelSimulationWheelData> m_wheelData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_instance = des.ReadClassPointer<hkpVehicleInstance>(br);
            br.ReadUInt64();
            m_slipDamping = br.ReadSingle();
            m_impulseScaling = br.ReadSingle();
            m_maxImpulse = br.ReadSingle();
            m_takeDynamicVelocity = br.ReadSingle();
            m_curbDamping = br.ReadSingle();
            br.ReadUInt32();
            m_wheelData = des.ReadClassArray<hkpVehiclePerWheelSimulationWheelData>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(0);
            s.WriteClassPointer<hkpVehicleInstance>(bw, m_instance);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_slipDamping);
            bw.WriteSingle(m_impulseScaling);
            bw.WriteSingle(m_maxImpulse);
            bw.WriteSingle(m_takeDynamicVelocity);
            bw.WriteSingle(m_curbDamping);
            bw.WriteUInt32(0);
            s.WriteClassArray<hkpVehiclePerWheelSimulationWheelData>(bw, m_wheelData);
        }
    }
}
