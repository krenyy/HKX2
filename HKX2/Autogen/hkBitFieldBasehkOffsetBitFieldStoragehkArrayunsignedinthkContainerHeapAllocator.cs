using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkBitFieldBasehkOffsetBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public hkOffsetBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator m_storage;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_storage = new hkOffsetBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator();
            m_storage.Read(des, br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_storage.Write(s, bw);
        }
    }
}
