using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleDefaultAerodynamics : hkpVehicleAerodynamics
    {
        public override uint Signature { get => 0; }
        
        public float m_airDensity;
        public float m_frontalArea;
        public float m_dragCoefficient;
        public float m_liftCoefficient;
        public Vector4 m_extraGravityws;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_airDensity = br.ReadSingle();
            m_frontalArea = br.ReadSingle();
            m_dragCoefficient = br.ReadSingle();
            m_liftCoefficient = br.ReadSingle();
            br.ReadUInt32();
            m_extraGravityws = des.ReadVector4(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_airDensity);
            bw.WriteSingle(m_frontalArea);
            bw.WriteSingle(m_dragCoefficient);
            bw.WriteSingle(m_liftCoefficient);
            bw.WriteUInt32(0);
            s.WriteVector4(bw, m_extraGravityws);
        }
    }
}
