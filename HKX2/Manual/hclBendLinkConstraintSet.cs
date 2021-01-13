using System.Collections.Generic;

namespace HKX2
{
    public partial class hclBendLinkConstraintSet : hclConstraintSet
    {
        public override uint Signature => 0x26824757;

        public List<hclBendLinkConstraintSetLink> m_links;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_links = des.ReadClassArray<hclBendLinkConstraintSetLink>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_links);
        }
    }
}
