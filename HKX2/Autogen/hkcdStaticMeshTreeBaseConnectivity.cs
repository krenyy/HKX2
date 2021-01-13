using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdStaticMeshTreeBaseConnectivity : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public List<hkcdStaticMeshTreeBaseConnectivitySectionHeader> m_headers;
        public List<byte> m_localLinks;
        public List<uint> m_globalLinks;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_headers = des.ReadClassArray<hkcdStaticMeshTreeBaseConnectivitySectionHeader>(br);
            m_localLinks = des.ReadByteArray(br);
            m_globalLinks = des.ReadUInt32Array(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassArray<hkcdStaticMeshTreeBaseConnectivitySectionHeader>(bw, m_headers);
            s.WriteByteArray(bw, m_localLinks);
            s.WriteUInt32Array(bw, m_globalLinks);
        }
    }
}
