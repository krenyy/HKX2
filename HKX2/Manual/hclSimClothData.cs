using System.Collections.Generic;

namespace HKX2
{
    public class hclSimClothData : hkReferencedObject
    {
        public override uint Signature => 0xE6105187;

        public hclSimClothDataOverridableSimulationInfo m_simulationInfo;
        public string m_name;
        public List<hclSimClothDataParticleData> m_particleDatas;
        public List<ushort> m_fixedParticles;
        public List<ushort> m_triangleIndices;
        public List<byte> m_triangleFlips;
        public float m_totalMass;
        public hclSimClothDataCollidableTransformMap m_collidableTransformMap;
        public List<hclCollidable> m_perInstanceCollidables;
        public List<hclConstraintSet> m_staticConstraintSets;
        public List<hclConstraintSet> m_antiPinchConstraintSets;
        public List<hclSimClothPose> m_simClothPoses;
        public List<hclAction> m_actions;
        public List<uint> m_staticCollisionMasks;
        public List<bool> m_perParticlePinchDetectionEnabledFlags;
        public List<hclSimClothDataCollidablePinchingData> m_collidablePinchingDatas;
        public ushort m_minPinchedParticleIndex;
        public ushort m_maxPinchedParticleIndex;
        public uint m_maxCollisionPairs;
        public float m_maxParticleRadius;
        public hclSimClothDataLandscapeCollisionData m_landscapeCollisionData;
        public uint m_numLandscapeCollidableParticles;
        public bool m_doNormals;
        public hclSimClothDataTransferMotionData m_transferMotionData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_simulationInfo = new hclSimClothDataOverridableSimulationInfo();
            m_simulationInfo.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_particleDatas = des.ReadClassArray<hclSimClothDataParticleData>(br);
            m_fixedParticles = des.ReadUInt16Array(br);
            m_triangleIndices = des.ReadUInt16Array(br);
            m_triangleFlips = des.ReadByteArray(br);
            m_totalMass = br.ReadSingle();

            if (des._header.PointerSize == 8)
            {
                br.ReadUInt32();
            }

            m_collidableTransformMap = new hclSimClothDataCollidableTransformMap();
            m_collidableTransformMap.Read(des, br);
            m_perInstanceCollidables = des.ReadClassPointerArray<hclCollidable>(br);
            m_staticConstraintSets = des.ReadClassPointerArray<hclConstraintSet>(br);
            m_antiPinchConstraintSets = des.ReadClassPointerArray<hclConstraintSet>(br);
            m_simClothPoses = des.ReadClassPointerArray<hclSimClothPose>(br);
            m_actions = des.ReadClassPointerArray<hclAction>(br);
            m_staticCollisionMasks = des.ReadUInt32Array(br);
            m_perParticlePinchDetectionEnabledFlags = des.ReadBooleanArray(br);
            m_collidablePinchingDatas = des.ReadClassArray<hclSimClothDataCollidablePinchingData>(br);
            m_minPinchedParticleIndex = br.ReadUInt16();
            m_maxPinchedParticleIndex = br.ReadUInt16();
            m_maxCollisionPairs = br.ReadUInt32();
            m_maxParticleRadius = br.ReadSingle();
            m_landscapeCollisionData = new hclSimClothDataLandscapeCollisionData();
            m_landscapeCollisionData.Read(des, br);
            m_numLandscapeCollidableParticles = br.ReadUInt32();
            m_doNormals = br.ReadBoolean();
            br.ReadByte();
            br.ReadUInt16();
            m_transferMotionData = new hclSimClothDataTransferMotionData();
            m_transferMotionData.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_simulationInfo.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteClassArray(bw, m_particleDatas);
            s.WriteUInt16Array(bw, m_fixedParticles);
            s.WriteUInt16Array(bw, m_triangleIndices);
            s.WriteByteArray(bw, m_triangleFlips);
            bw.WriteSingle(m_totalMass);

            if (s._header.PointerSize == 8)
            {
                bw.WriteUInt32(0);
            }

            m_collidableTransformMap.Write(s, bw);
            s.WriteClassPointerArray(bw, m_perInstanceCollidables);
            s.WriteClassPointerArray(bw, m_staticConstraintSets);
            s.WriteClassPointerArray(bw, m_antiPinchConstraintSets);
            s.WriteClassPointerArray(bw, m_simClothPoses);
            s.WriteClassPointerArray(bw, m_actions);
            s.WriteUInt32Array(bw, m_staticCollisionMasks);
            s.WriteBooleanArray(bw, m_perParticlePinchDetectionEnabledFlags);
            s.WriteClassArray(bw, m_collidablePinchingDatas);
            bw.WriteUInt16(m_minPinchedParticleIndex);
            bw.WriteUInt16(m_maxPinchedParticleIndex);
            bw.WriteUInt32(m_maxCollisionPairs);
            bw.WriteSingle(m_maxParticleRadius);
            m_landscapeCollisionData.Write(s, bw);
            bw.WriteUInt32(m_numLandscapeCollidableParticles);
            bw.WriteBoolean(m_doNormals);
            bw.WriteByte(0);
            bw.WriteUInt16(0);
            m_transferMotionData.Write(s, bw);
        }
    }
}
