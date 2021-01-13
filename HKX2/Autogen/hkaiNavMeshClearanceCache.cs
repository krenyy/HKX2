using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiNavMeshClearanceCache : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public float m_clearanceCeiling;
        public float m_clearanceIntToRealMultiplier;
        public float m_clearanceRealToIntMultiplier;
        public List<uint> m_faceOffsets;
        public List<byte> m_edgePairClearances;
        public int m_unusedEdgePairElements;
        public List<hkaiNavMeshClearanceCacheMcpDataInteger> m_mcpData;
        public List<byte> m_vertexClearances;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_clearanceCeiling = br.ReadSingle();
            m_clearanceIntToRealMultiplier = br.ReadSingle();
            m_clearanceRealToIntMultiplier = br.ReadSingle();
            m_faceOffsets = des.ReadUInt32Array(br);
            m_edgePairClearances = des.ReadByteArray(br);
            m_unusedEdgePairElements = br.ReadInt32();
            br.ReadUInt32();
            m_mcpData = des.ReadClassArray<hkaiNavMeshClearanceCacheMcpDataInteger>(br);
            m_vertexClearances = des.ReadByteArray(br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_clearanceCeiling);
            bw.WriteSingle(m_clearanceIntToRealMultiplier);
            bw.WriteSingle(m_clearanceRealToIntMultiplier);
            s.WriteUInt32Array(bw, m_faceOffsets);
            s.WriteByteArray(bw, m_edgePairClearances);
            bw.WriteInt32(m_unusedEdgePairElements);
            bw.WriteUInt32(0);
            s.WriteClassArray<hkaiNavMeshClearanceCacheMcpDataInteger>(bw, m_mcpData);
            s.WriteByteArray(bw, m_vertexClearances);
        }
    }
}
