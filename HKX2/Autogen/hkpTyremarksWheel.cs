using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpTyremarksWheel : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public int m_currentPosition;
        public int m_numPoints;
        public List<hkpTyremarkPoint> m_tyremarkPoints;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_currentPosition = br.ReadInt32();
            m_numPoints = br.ReadInt32();
            br.ReadUInt32();
            m_tyremarkPoints = des.ReadClassArray<hkpTyremarkPoint>(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_currentPosition);
            bw.WriteInt32(m_numPoints);
            bw.WriteUInt32(0);
            s.WriteClassArray<hkpTyremarkPoint>(bw, m_tyremarkPoints);
        }
    }
}
