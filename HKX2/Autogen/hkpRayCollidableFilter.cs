using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpRayCollidableFilter : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.ReadUInt64();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
        }
    }
}
