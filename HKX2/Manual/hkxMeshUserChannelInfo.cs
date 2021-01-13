namespace HKX2
{
    public partial class hkxMeshUserChannelInfo : hkxAttributeHolder
    {
        public override uint Signature => 0x0;
        
        public string m_name;
        public string m_className;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_className = des.ReadStringPointer(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteStringPointer(bw, m_className);
        }
    }
}
