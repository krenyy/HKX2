using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpDefaultWorldMemoryWatchDog : hkWorldMemoryAvailableWatchDog
    {
        public override uint Signature { get => 0; }
        
        public int m_freeHeapMemoryRequested;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_freeHeapMemoryRequested = br.ReadInt32();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_freeHeapMemoryRequested);
        }
    }
}
