using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkLocalFrameGroup : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
        }
    }
}
