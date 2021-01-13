using System.Numerics;

namespace HKX2
{
    public partial class hkpStaticCompoundShapeInstance : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public Matrix4x4 m_transform;
        public hkpShape m_shape;
        public uint m_filterInfo;
        public uint m_childFilterInfoMask;
        public ulong m_userData;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transform = des.ReadQSTransform(br);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_filterInfo = br.ReadUInt32();
            m_childFilterInfoMask = br.ReadUInt32();
            m_userData = br.ReadUSize();

            if (des._header.PointerSize == 8)
            {
                br.ReadUInt64();
            }
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteQSTransform(bw, m_transform);
            s.WriteClassPointer<hkpShape>(bw, m_shape);
            bw.WriteUInt32(m_filterInfo);
            bw.WriteUInt32(m_childFilterInfoMask);
            bw.WriteUSize(m_userData);

            if (s._header.PointerSize == 8)
            {
                bw.WriteUInt64(0);
            }
        }
    }
}
