namespace HKX2
{
    public partial class hkUFloat8 : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public byte m_value;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = br.ReadByte();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteByte(m_value);
        }
    }
}
