namespace HKX2
{
    public partial class hkaiDirectedGraphExplicitCostEdge : IHavokObject
    {
        public virtual uint Signature => 0;

        public short m_cost;
        public EdgeBits m_flags;
        public uint m_target;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_cost = br.ReadInt16();
            m_flags = (EdgeBits) br.ReadUInt16();
            m_target = br.ReadUInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_cost);
            bw.WriteUInt16((ushort) m_flags);
            bw.WriteUInt32(m_target);
        }
    }
}
