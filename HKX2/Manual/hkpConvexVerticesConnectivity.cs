using System.Collections.Generic;

namespace HKX2
{
    public partial class hkpConvexVerticesConnectivity : hkReferencedObject
    {
        public override uint Signature => 0x0;

        public List<ushort> m_vertexIndices;
        public List<byte> m_numVerticesPerFace;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexIndices = des.ReadUInt16Array(br);
            m_numVerticesPerFace = des.ReadByteArray(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteUInt16Array(bw, m_vertexIndices);
            s.WriteByteArray(bw, m_numVerticesPerFace);
        }
    }
}
