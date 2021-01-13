namespace HKX2
{
    public partial class hkaAnnotationTrackAnnotation : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public float m_time;
        public string m_text;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            m_text = des.ReadStringPointer(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            s.WriteStringPointer(bw, m_text);
        }
    }
}
