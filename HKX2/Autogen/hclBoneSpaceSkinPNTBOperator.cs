using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hclBoneSpaceSkinPNTBOperator : hclBoneSpaceSkinOperator
    {
        public override uint Signature { get => 0; }
        
        public List<hclBoneSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNTBs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockPNTB>(br);
            m_localUnpackedPNTBs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockUnpackedPNTB>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray<hclBoneSpaceDeformerLocalBlockPNTB>(bw, m_localPNTBs);
            s.WriteClassArray<hclBoneSpaceDeformerLocalBlockUnpackedPNTB>(bw, m_localUnpackedPNTBs);
        }
    }
}
