namespace HKX2
{
    public partial class hclSphereShape : hclShape
    {
        public override uint Signature => 0xD779F2C5;

        public hkSphere m_sphere;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sphere = new hkSphere();
            m_sphere.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_sphere.Write(s, bw);
        }
    }
}
