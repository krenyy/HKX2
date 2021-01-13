using System.Collections.Generic;

namespace HKX2
{
    public partial class hkBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public List<uint> m_words;
        public int m_numBits;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_words = des.ReadUInt32Array(br);
            m_numBits = br.ReadInt32();

            if (des._header.PointerSize == 8)
            {
                br.ReadUInt32();
            }
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteUInt32Array(bw, m_words);
            bw.WriteInt32(m_numBits);

            if (s._header.PointerSize == 8)
            {
                bw.WriteUInt32(0);
            }
        }
    }
}
