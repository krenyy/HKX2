using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiNavMeshClearanceCacheMcpDataInteger : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public byte m_interpolant;
        public byte m_clearance;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_interpolant = br.ReadByte();
            m_clearance = br.ReadByte();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteByte(m_interpolant);
            bw.WriteByte(m_clearance);
        }
    }
}
