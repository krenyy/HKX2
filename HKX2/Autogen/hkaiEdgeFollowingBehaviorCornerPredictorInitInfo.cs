using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiEdgeFollowingBehaviorCornerPredictorInitInfo : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public Vector4 m_positionLocal;
        public Vector4 m_forwardVectorLocal;
        public Vector4 m_upLocal;
        public int m_positionSectionIndex;
        public int m_nextEdgeIndex;
        public bool m_nextIsLeft;
        public bool m_hasInfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_positionLocal = des.ReadVector4(br);
            m_forwardVectorLocal = des.ReadVector4(br);
            m_upLocal = des.ReadVector4(br);
            m_positionSectionIndex = br.ReadInt32();
            m_nextEdgeIndex = br.ReadInt32();
            m_nextIsLeft = br.ReadBoolean();
            m_hasInfo = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVector4(bw, m_positionLocal);
            s.WriteVector4(bw, m_forwardVectorLocal);
            s.WriteVector4(bw, m_upLocal);
            bw.WriteInt32(m_positionSectionIndex);
            bw.WriteInt32(m_nextEdgeIndex);
            bw.WriteBoolean(m_nextIsLeft);
            bw.WriteBoolean(m_hasInfo);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
