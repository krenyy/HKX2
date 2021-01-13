using System.Numerics;

namespace HKX2
{
    public partial class hkaSkeletonMapperDataChainMapping : IHavokObject
    {
        public virtual uint Signature => 0x0;

        public short m_startBoneA;
        public short m_endBoneA;
        public short m_startBoneB;
        public short m_endBoneB;
        public Matrix4x4 m_startAFromBTransform;
        public Matrix4x4 m_endAFromBTransform;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_startBoneA = br.ReadInt16();
            m_endBoneA = br.ReadInt16();
            m_startBoneB = br.ReadInt16();
            m_endBoneB = br.ReadInt16();
            br.Pad(16);
            m_startAFromBTransform = des.ReadQSTransform(br);
            m_endAFromBTransform = des.ReadQSTransform(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_startBoneA);
            bw.WriteInt16(m_endBoneA);
            bw.WriteInt16(m_startBoneB);
            bw.WriteInt16(m_endBoneB);
            bw.Pad(16);
            s.WriteQSTransform(bw, m_startAFromBTransform);
            s.WriteQSTransform(bw, m_endAFromBTransform);
        }
    }
}
