using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdDynamicTreeDynamicStorage0hkcdDynamicTreeAnisotropicMetrichkcdDynamicTreeCodecRawUint : hkcdDynamicTreeAnisotropicMetric
    {
        public override uint Signature { get => 0; }
        
        public List<hkcdDynamicTreeCodecRawUint> m_nodes;
        public uint m_firstFree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nodes = des.ReadClassArray<hkcdDynamicTreeCodecRawUint>(br);
            m_firstFree = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray<hkcdDynamicTreeCodecRawUint>(bw, m_nodes);
            bw.WriteUInt32(m_firstFree);
            bw.WriteUInt32(0);
        }
    }
}
