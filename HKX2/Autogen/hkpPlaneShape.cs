using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpPlaneShape : hkpHeightFieldShape
    {
        public override uint Signature { get => 0; }
        
        public Vector4 m_plane;
        public Vector4 m_aabbCenter;
        public Vector4 m_aabbHalfExtents;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_plane = des.ReadVector4(br);
            m_aabbCenter = des.ReadVector4(br);
            m_aabbHalfExtents = des.ReadVector4(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(0);
            s.WriteVector4(bw, m_plane);
            s.WriteVector4(bw, m_aabbCenter);
            s.WriteVector4(bw, m_aabbHalfExtents);
        }
    }
}
