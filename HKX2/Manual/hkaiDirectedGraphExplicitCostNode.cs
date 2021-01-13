namespace HKX2
{
    public partial class hkaiDirectedGraphExplicitCostNode : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public int m_startEdgeIndex;
        public int m_numEdges;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_startEdgeIndex = br.ReadInt32();
            m_numEdges = br.ReadInt32();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_startEdgeIndex);
            bw.WriteInt32(m_numEdges);
        }
    }
}
