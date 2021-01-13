using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdPlanarCsgOperand : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public hkcdPlanarSolid m_solid;
        public hkcdPlanarGeometry m_danglingGeometry;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_solid = des.ReadClassPointer<hkcdPlanarSolid>(br);
            m_danglingGeometry = des.ReadClassPointer<hkcdPlanarGeometry>(br);
            br.ReadUInt64();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer<hkcdPlanarSolid>(bw, m_solid);
            s.WriteClassPointer<hkcdPlanarGeometry>(bw, m_danglingGeometry);
            bw.WriteUInt64(0);
        }
    }
}
