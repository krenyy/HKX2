using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdStaticMeshTreeBaseEdge : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public uint m_keyAndIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_keyAndIndex = br.ReadUInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_keyAndIndex);
        }
    }
}
