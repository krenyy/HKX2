using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkcdDynamicAabbTree : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public hkcdDynamicTreeDefaultTree48Storage m_treePtr;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_treePtr = des.ReadClassPointer<hkcdDynamicTreeDefaultTree48Storage>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer<hkcdDynamicTreeDefaultTree48Storage>(bw, m_treePtr);
        }
    }
}
