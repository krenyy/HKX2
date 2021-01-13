using System.Collections.Generic;

namespace HKX2
{
    public partial class hkcdStaticTreeDynamicStoragehkcdStaticTreeCodec3Axis6 : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public List<hkcdStaticTreeCodec3Axis6> m_nodes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_nodes = des.ReadClassArray<hkcdStaticTreeCodec3Axis6>(br);
            br.Pad(16);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassArray(bw, m_nodes);
            bw.Pad(16);
        }
    }
}
