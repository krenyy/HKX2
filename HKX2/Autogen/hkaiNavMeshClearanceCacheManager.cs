using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CachingOption
    {
        CACHE = 0,
        USE_UNFILTERED = 1,
        DO_NOT_CACHE = 2,
        NOT_SET = -1,
    }
    
    public enum DefaultCachingOption
    {
        DCO_WARN_AND_TREAT_AS_UNFILTERED = 0,
        DCO_TREAT_AS_UNFILTERED = 1,
        DCO_DO_NOT_CACHE = 2,
        DCO_ASSERT = 3,
    }
    
    public partial class hkaiNavMeshClearanceCacheManager : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public List<hkaiNavMeshClearanceCacheManagerRegistration> m_registrations;
        public List<hkaiNavMeshClearanceCacheManagerCacheInfo> m_cacheInfos;
        public DefaultCachingOption m_defaultOption;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_registrations = des.ReadClassArray<hkaiNavMeshClearanceCacheManagerRegistration>(br);
            m_cacheInfos = des.ReadClassArray<hkaiNavMeshClearanceCacheManagerCacheInfo>(br);
            m_defaultOption = (DefaultCachingOption)br.ReadInt32();
            br.ReadUInt32();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray<hkaiNavMeshClearanceCacheManagerRegistration>(bw, m_registrations);
            s.WriteClassArray<hkaiNavMeshClearanceCacheManagerCacheInfo>(bw, m_cacheInfos);
            bw.WriteInt32((int)m_defaultOption);
            bw.WriteUInt32(0);
        }
    }
}
