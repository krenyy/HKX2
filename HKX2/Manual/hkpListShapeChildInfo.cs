namespace HKX2
{
    public partial class hkpListShapeChildInfo : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public hkpShape m_shape;
        public uint m_collisionFilterInfo;
        public ushort m_shapeInfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_collisionFilterInfo = br.ReadUInt32();
            m_shapeInfo = br.ReadUInt16();
            br.ReadInt16(); // shapeSize
            br.ReadInt32(); // numChildShapes
            br.Pad(16);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer<hkpShape>(bw, m_shape);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteUInt16(m_shapeInfo);
            bw.WriteInt16(0); // shapeSize
            bw.WriteInt32(0); // numChildShapes
            bw.Pad(16);
        }
    }
}
