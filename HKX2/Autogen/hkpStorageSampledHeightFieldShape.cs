using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpStorageSampledHeightFieldShape : hkpSampledHeightFieldShape
    {
        public override uint Signature { get => 0; }
        
        public List<float> m_storage;
        public bool m_triangleFlip;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadSingleArray(br);
            m_triangleFlip = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_storage);
            bw.WriteBoolean(m_triangleFlip);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
