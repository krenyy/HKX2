using System.Collections.Generic;

namespace HKX2
{
    public partial class hkxAttributeHolder : hkReferencedObject
    {
        public override uint Signature => 0x0;
        
        public List<hkxAttributeGroup> m_attributeGroups;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_attributeGroups = des.ReadClassArray<hkxAttributeGroup>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_attributeGroups);
        }
    }
}
