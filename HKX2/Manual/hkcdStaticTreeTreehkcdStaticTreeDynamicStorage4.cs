namespace HKX2
{
    public partial class hkcdStaticTreeTreehkcdStaticTreeDynamicStorage4 : hkcdStaticTreeDynamicStorage4
    {
        public override uint Signature => 0x0;

        public hkAabb m_domain;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_domain = new hkAabb();
            m_domain.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_domain.Write(s, bw);
        }
    }
}
