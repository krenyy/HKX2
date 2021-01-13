using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiNavMeshGenerationSettingsRegionPruningSettingsRegionConnection : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public Vector4 m_a;
        public Vector4 m_b;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_a = des.ReadVector4(br);
            m_b = des.ReadVector4(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVector4(bw, m_a);
            s.WriteVector4(bw, m_b);
        }
    }
}
