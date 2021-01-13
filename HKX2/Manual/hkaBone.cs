namespace HKX2
{
    public partial class hkaBone : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public string m_name;
        public bool m_lockTranslation;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_lockTranslation = br.ReadBoolean();
            br.ReadByte();
            br.ReadUInt16();

            if (des._header.PointerSize == 8)
            {
                br.ReadUInt32();
            }
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_name);
            bw.WriteBoolean(m_lockTranslation);
            bw.WriteByte(0);
            bw.WriteUInt16(0);

            if (s._header.PointerSize == 8)
            {
                bw.WriteUInt32(0);
            }
        }
    }
}
