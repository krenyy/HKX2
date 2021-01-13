using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiNavMeshClearanceCacheManagerCacheInfo : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public float m_clearanceCeiling;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_clearanceCeiling = br.ReadSingle();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_clearanceCeiling);
        }
    }
}
