using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpUnaryAction : hkpAction
    {
        public override uint Signature { get => 0; }
        
        public hkpEntity m_entity;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_entity = des.ReadClassPointer<hkpEntity>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer<hkpEntity>(bw, m_entity);
        }
    }
}
