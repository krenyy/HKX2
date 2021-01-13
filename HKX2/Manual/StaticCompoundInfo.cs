using System.Collections.Generic;

namespace HKX2
{
    public partial class StaticCompoundInfo : IHavokObject
    {
        public virtual uint Signature => 0x5115A202;

        public uint m_Offset;
        public List<ActorInfo> m_ActorInfo;
        public List<ShapeInfo> m_ShapeInfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_Offset = br.ReadUInt32();
            m_ActorInfo = des.ReadClassArray<ActorInfo>(br);
            m_ShapeInfo = des.ReadClassArray<ShapeInfo>(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_Offset);
            s.WriteClassArray<ActorInfo>(bw, m_ActorInfo);
            s.WriteClassArray<ShapeInfo>(bw, m_ShapeInfo);
        }
    }
}
