using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpTriSampledHeightFieldBvTreeShape : hkpBvTreeShape
    {
        public override uint Signature { get => 0; }
        
        public hkpSingleShapeContainer m_childContainer;
        public bool m_wantAabbRejectionTest;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childContainer = new hkpSingleShapeContainer();
            m_childContainer.Read(des, br);
            br.ReadUInt32();
            m_wantAabbRejectionTest = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_childContainer.Write(s, bw);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_wantAabbRejectionTest);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
