using System.Numerics;

namespace HKX2
{
    public partial class hclPlaneShape : hclShape
    {
        public override uint Signature => 0x4D60B010;

        public Vector4 m_planeEquation;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Pad(16);
            m_planeEquation = des.ReadVector4(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Pad(16);
            s.WriteVector4(bw, m_planeEquation);
        }
    }
}
