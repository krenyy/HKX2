using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdStaticMeshTreeBaseConnectivitySectionHeader : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public uint m_baseLocal;
        public uint m_baseGlobal;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_baseLocal = br.ReadUInt32();
            m_baseGlobal = br.ReadUInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_baseLocal);
            bw.WriteUInt32(m_baseGlobal);
        }
    }
}
