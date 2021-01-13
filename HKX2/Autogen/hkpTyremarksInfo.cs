using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpTyremarksInfo : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public float m_minTyremarkEnergy;
        public float m_maxTyremarkEnergy;
        public List<hkpTyremarksWheel> m_tyremarksWheel;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_minTyremarkEnergy = br.ReadSingle();
            m_maxTyremarkEnergy = br.ReadSingle();
            br.ReadUInt32();
            m_tyremarksWheel = des.ReadClassPointerArray<hkpTyremarksWheel>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_minTyremarkEnergy);
            bw.WriteSingle(m_maxTyremarkEnergy);
            bw.WriteUInt32(0);
            s.WriteClassPointerArray<hkpTyremarksWheel>(bw, m_tyremarksWheel);
        }
    }
}
