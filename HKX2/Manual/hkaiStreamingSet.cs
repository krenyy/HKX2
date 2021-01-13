using System.Collections.Generic;

namespace HKX2
{
    public partial class hkaiStreamingSet : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public uint m_thisUid;
        public uint m_oppositeUid;
        public List<hkaiStreamingSetNavMeshConnection> m_meshConnections;
        public List<hkaiStreamingSetGraphConnection> m_graphConnections;
        public List<hkaiStreamingSetVolumeConnection> m_volumeConnections;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_thisUid = br.ReadUInt32();
            m_oppositeUid = br.ReadUInt32();
            m_meshConnections = des.ReadClassArray<hkaiStreamingSetNavMeshConnection>(br);
            m_graphConnections = des.ReadClassArray<hkaiStreamingSetGraphConnection>(br);
            m_volumeConnections = des.ReadClassArray<hkaiStreamingSetVolumeConnection>(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_thisUid);
            bw.WriteUInt32(m_oppositeUid);
            s.WriteClassArray(bw, m_meshConnections);
            s.WriteClassArray(bw, m_graphConnections);
            s.WriteClassArray(bw, m_volumeConnections);
        }
    }
}
