using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum EdgeFilterType
    {
        EDGE_FILTER_DEFAULT = 0,
        EDGE_FILTER_USER = 1,
    }
    
    public partial class hkaiAstarEdgeFilter : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public EdgeFilterType m_type;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (EdgeFilterType)br.ReadByte();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte((byte)m_type);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
