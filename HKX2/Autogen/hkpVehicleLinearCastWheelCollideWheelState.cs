using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleLinearCastWheelCollideWheelState : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public hkpAabbPhantom m_phantom;
        public hkpShape m_shape;
        public Matrix4x4 m_transform;
        public Vector4 m_to;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_phantom = des.ReadClassPointer<hkpAabbPhantom>(br);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_transform = des.ReadTransform(br);
            m_to = des.ReadVector4(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer<hkpAabbPhantom>(bw, m_phantom);
            s.WriteClassPointer<hkpShape>(bw, m_shape);
            s.WriteTransform(bw, m_transform);
            s.WriteVector4(bw, m_to);
        }
    }
}
