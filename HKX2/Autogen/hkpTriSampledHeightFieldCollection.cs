using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpTriSampledHeightFieldCollection : hkpShapeCollection
    {
        public override uint Signature { get => 0; }
        
        public hkpSampledHeightFieldShape m_heightfield;
        public float m_radius;
        public List<ushort> m_weldingInfo;
        public Vector4 m_triangleExtrusion;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_heightfield = des.ReadClassPointer<hkpSampledHeightFieldShape>(br);
            br.ReadUInt32();
            m_radius = br.ReadSingle();
            m_weldingInfo = des.ReadUInt16Array(br);
            br.ReadUInt64();
            m_triangleExtrusion = des.ReadVector4(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer<hkpSampledHeightFieldShape>(bw, m_heightfield);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_radius);
            s.WriteUInt16Array(bw, m_weldingInfo);
            bw.WriteUInt64(0);
            s.WriteVector4(bw, m_triangleExtrusion);
        }
    }
}
