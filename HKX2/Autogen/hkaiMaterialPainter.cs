using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiMaterialPainter : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public int m_material;
        public hkaiVolume m_volume;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_material = br.ReadInt32();
            m_volume = des.ReadClassPointer<hkaiVolume>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_material);
            s.WriteClassPointer<hkaiVolume>(bw, m_volume);
        }
    }
}
