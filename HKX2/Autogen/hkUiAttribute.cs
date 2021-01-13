using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkUiAttribute : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public bool m_visible;
        public bool m_editable;
        public Types m_hideCriteria;
        public string m_label;
        public string m_group;
        public string m_hideBaseClassMembers;
        public bool m_endGroup;
        public bool m_endGroup2;
        public bool m_advanced;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_visible = br.ReadBoolean();
            m_editable = br.ReadBoolean();
            m_hideCriteria = (Types)br.ReadSByte();
            br.ReadUInt32();
            br.ReadByte();
            m_label = des.ReadStringPointer(br);
            m_group = des.ReadStringPointer(br);
            m_hideBaseClassMembers = des.ReadStringPointer(br);
            m_endGroup = br.ReadBoolean();
            m_endGroup2 = br.ReadBoolean();
            m_advanced = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadByte();
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_visible);
            bw.WriteBoolean(m_editable);
            bw.WriteSByte((sbyte)m_hideCriteria);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
            s.WriteStringPointer(bw, m_label);
            s.WriteStringPointer(bw, m_group);
            s.WriteStringPointer(bw, m_hideBaseClassMembers);
            bw.WriteBoolean(m_endGroup);
            bw.WriteBoolean(m_endGroup2);
            bw.WriteBoolean(m_advanced);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
