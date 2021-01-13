using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultVelocityDamper : hkpVehicleVelocityDamper
    {
        public override uint Signature { get => 0; }
        
        public float m_normalSpinDamping;
        public float m_collisionSpinDamping;
        public float m_collisionThreshold;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_normalSpinDamping = br.ReadSingle();
            m_collisionSpinDamping = br.ReadSingle();
            m_collisionThreshold = br.ReadSingle();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_normalSpinDamping);
            bw.WriteSingle(m_collisionSpinDamping);
            bw.WriteSingle(m_collisionThreshold);
        }
    }
}
