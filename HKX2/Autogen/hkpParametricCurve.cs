using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpParametricCurve : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
        }
    }
}
