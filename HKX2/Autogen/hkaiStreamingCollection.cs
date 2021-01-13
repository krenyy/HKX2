using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiStreamingCollection : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public bool m_isTemporary;
        public hkcdDynamicAabbTree m_tree;
        public List<hkaiStreamingCollectionInstanceInfo> m_instances;
        public hkaiNavMeshClearanceCacheManager m_clearanceCacheManager;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isTemporary = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
            m_tree = des.ReadClassPointer<hkcdDynamicAabbTree>(br);
            m_instances = des.ReadClassArray<hkaiStreamingCollectionInstanceInfo>(br);
            m_clearanceCacheManager = des.ReadClassPointer<hkaiNavMeshClearanceCacheManager>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isTemporary);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            s.WriteClassPointer<hkcdDynamicAabbTree>(bw, m_tree);
            s.WriteClassArray<hkaiStreamingCollectionInstanceInfo>(bw, m_instances);
            s.WriteClassPointer<hkaiNavMeshClearanceCacheManager>(bw, m_clearanceCacheManager);
        }
    }
}
