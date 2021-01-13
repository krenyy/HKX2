using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkaiPathFollowingProperties : hkReferencedObject
    {
        public override uint Signature { get => 0; }
        
        public float m_repathDistance;
        public int m_incompleteRepathSegments;
        public float m_searchPathLimit;
        public float m_desiredSpeedAtEnd;
        public float m_goalDistTolerance;
        public float m_userEdgeTolerance;
        public bool m_repairPaths;
        public bool m_setManualControlOnUserEdges;
        public bool m_pullThroughInternalVertices;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_repathDistance = br.ReadSingle();
            m_incompleteRepathSegments = br.ReadInt32();
            m_searchPathLimit = br.ReadSingle();
            m_desiredSpeedAtEnd = br.ReadSingle();
            m_goalDistTolerance = br.ReadSingle();
            m_userEdgeTolerance = br.ReadSingle();
            m_repairPaths = br.ReadBoolean();
            m_setManualControlOnUserEdges = br.ReadBoolean();
            m_pullThroughInternalVertices = br.ReadBoolean();
            br.ReadByte();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_repathDistance);
            bw.WriteInt32(m_incompleteRepathSegments);
            bw.WriteSingle(m_searchPathLimit);
            bw.WriteSingle(m_desiredSpeedAtEnd);
            bw.WriteSingle(m_goalDistTolerance);
            bw.WriteSingle(m_userEdgeTolerance);
            bw.WriteBoolean(m_repairPaths);
            bw.WriteBoolean(m_setManualControlOnUserEdges);
            bw.WriteBoolean(m_pullThroughInternalVertices);
            bw.WriteByte(0);
        }
    }
}
