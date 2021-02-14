namespace HKX2
{
    public partial class hkpCdBody : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public hkpShape m_shape;
        public uint m_shapeKey;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_shapeKey = br.ReadUInt32();
            des.ReadEmptyPointer(br); // motion
            des.ReadEmptyPointer(br); // parent
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_shape);
            bw.WriteUInt32(m_shapeKey);
            s.WriteVoidPointer(bw); // motion
            s.WriteVoidPointer(bw); // parent
        }
    }
}
