namespace HKX2
{
    public partial class hkpSingleShapeContainer : hkpShapeContainer
    {
        public override uint Signature => 0x0;

        public hkpShape m_childShape;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childShape = des.ReadClassPointer<hkpShape>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_childShape);
        }
    }
}
