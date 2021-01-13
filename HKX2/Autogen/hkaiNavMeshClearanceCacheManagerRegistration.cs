using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiNavMeshClearanceCacheManagerRegistration : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public ulong m_id;
        public uint m_info;
        public uint m_infoMask;
        public byte m_cacheIdentifier;
        public hkaiAstarEdgeFilter m_filter;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadUInt64();
            m_info = br.ReadUInt32();
            m_infoMask = br.ReadUInt32();
            m_cacheIdentifier = br.ReadByte();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
            m_filter = des.ReadClassPointer<hkaiAstarEdgeFilter>(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_id);
            bw.WriteUInt32(m_info);
            bw.WriteUInt32(m_infoMask);
            bw.WriteByte(m_cacheIdentifier);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            s.WriteClassPointer<hkaiAstarEdgeFilter>(bw, m_filter);
        }
    }
}
