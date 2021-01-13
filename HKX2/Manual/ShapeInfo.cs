namespace HKX2
{
    public partial class ShapeInfo : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public int m_ActorInfoIndex;
        public int m_InstanceId;
        public sbyte m_BodyGroup;
        public byte m_BodyLayerType;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_ActorInfoIndex = br.ReadInt32();
            m_InstanceId = br.ReadInt32();
            m_BodyGroup = br.ReadSByte();
            m_BodyLayerType = br.ReadByte();
            br.ReadUInt16();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_ActorInfoIndex);
            bw.WriteInt32(m_InstanceId);
            bw.WriteSByte(m_BodyGroup);
            bw.WriteByte(m_BodyLayerType);
            bw.WriteUInt16(0);
        }
    }
}
