namespace HKX2
{
    public partial class ActorInfo : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public uint m_HashId;
        public int m_SRTHash;
        public int m_ShapeInfoStart;
        public int m_ShapeInfoEnd;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_HashId = br.ReadUInt32();
            m_SRTHash = br.ReadInt32();
            m_ShapeInfoStart = br.ReadInt32();
            m_ShapeInfoEnd = br.ReadInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_HashId);
            bw.WriteInt32(m_SRTHash);
            bw.WriteInt32(m_ShapeInfoStart);
            bw.WriteInt32(m_ShapeInfoEnd);
        }
    }
}
