using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using HKX2;
using HKX2.Builders;
using ObjLoader.Loader.Loaders;

namespace CreateCollisionAndNavmesh
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            LoadResult res;
            using (FileStream fs = File.OpenRead(args[0]))
            {
                var fact = new ObjLoaderFactory();
                var loader = fact.Create();
                res = loader.Load(fs);
            }

            var verts = new List<Vector3>();
            var indices = new List<uint>();

            foreach (var vert in res.Vertices)
            {
                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
            }

            foreach (var idx in res.Groups[0].Faces)
            {
                indices.Add((uint) (idx[0].VertexIndex - 1));
                indices.Add((uint) (idx[1].VertexIndex - 1));
                indices.Add((uint) (idx[2].VertexIndex - 1));
            }

            var s = new PackFileSerializer();

            if (args[1] == "hkrb")
            {
                #region hkRootLevelContainer
                var root = new hkRootLevelContainer();
                root.m_namedVariants = new List<hkRootLevelContainerNamedVariant>();
                #endregion

                #region hkRootLevelContainerNamedVariant
                var namedVariant = new hkRootLevelContainerNamedVariant();
                namedVariant.m_name = "Physics Data";
                namedVariant.m_className = "hkpPhysicsData";
                root.m_namedVariants.Add(namedVariant);
                #endregion

                #region hkpPhysicsData
                var variant = new hkpPhysicsData();
                variant.m_worldCinfo = null;
                variant.m_systems = new List<hkpPhysicsSystem>();
                namedVariant.m_variant = variant;
                #endregion

                #region hkpPhysicsSystem
                var system = new hkpPhysicsSystem();
                system.m_rigidBodies = new List<hkpRigidBody>();
                system.m_constraints = new List<hkpConstraintInstance>();
                system.m_actions = new List<hkpAction>();
                system.m_phantoms = new List<hkpPhantom>();
                system.m_name = "Default Physics System";
                system.m_userData = 0;
                system.m_active = true;
                variant.m_systems.Add(system);
                #endregion

                #region hkpRigidBody
                var rigidBody = new hkpRigidBody();
                rigidBody.m_userData = 0;
                rigidBody.m_collidable = new hkpLinkedCollidable();
                rigidBody.m_collidable.m_shapeKey = 0xFFFFFFFF;
                rigidBody.m_collidable.m_forceCollideOntoPpu = 8;
                rigidBody.m_collidable.m_broadPhaseHandle = new hkpTypedBroadPhaseHandle();
                rigidBody.m_collidable.m_broadPhaseHandle.m_type = BroadPhaseType.BROAD_PHASE_ENTITY;
                rigidBody.m_collidable.m_broadPhaseHandle.m_objectQualityType = 0;
                rigidBody.m_collidable.m_broadPhaseHandle.m_collisionFilterInfo = 0x90000000;
                rigidBody.m_collidable.m_allowedPenetrationDepth = float.MaxValue;
                rigidBody.m_multiThreadCheck = new hkMultiThreadCheck();
                rigidBody.m_name = "Collision_IDK";
                rigidBody.m_properties = new List<hkSimpleProperty>();
                rigidBody.m_material = new hkpMaterial();
                rigidBody.m_material.m_responseType = ResponseType.RESPONSE_SIMPLE_CONTACT;
                rigidBody.m_material.m_rollingFrictionMultiplier = 0;
                rigidBody.m_material.m_friction = .5f;
                rigidBody.m_material.m_restitution = .4f;
                rigidBody.m_damageMultiplier = 1f;
                rigidBody.m_storageIndex = 0xFFFF;
                rigidBody.m_contactPointCallbackDelay = 0xFFFF;
                rigidBody.m_autoRemoveLevel = 0;
                rigidBody.m_numShapeKeysInContactPointProperties = 1;
                rigidBody.m_responseModifierFlags = 0;
                rigidBody.m_uid = 0xFFFFFFFF;
                rigidBody.m_spuCollisionCallback = new hkpEntitySpuCollisionCallback();
                rigidBody.m_spuCollisionCallback.m_eventFilter =
                    SpuCollisionCallbackEventFilter.SPU_SEND_CONTACT_POINT_ADDED_OR_PROCESS;
                rigidBody.m_spuCollisionCallback.m_userFilter = 1;
                rigidBody.m_motion = new hkpMaxSizeMotion();
                rigidBody.m_motion.m_type = MotionType.MOTION_FIXED;
                rigidBody.m_motion.m_deactivationIntegrateCounter = 15;
                rigidBody.m_motion.m_deactivationNumInactiveFrames_0 = 0xC000;
                rigidBody.m_motion.m_deactivationNumInactiveFrames_1 = 0xC000;
                rigidBody.m_motion.m_motionState = new hkMotionState();
                rigidBody.m_motion.m_motionState.m_transform = Matrix4x4.Identity;
                rigidBody.m_motion.m_motionState.m_sweptTransform_0 = new Vector4(.0f);
                rigidBody.m_motion.m_motionState.m_sweptTransform_1 = new Vector4(.0f);
                rigidBody.m_motion.m_motionState.m_sweptTransform_2 = new Vector4(.0f, .0f, .0f, .99999994f);
                rigidBody.m_motion.m_motionState.m_sweptTransform_3 = new Vector4(.0f, .0f, .0f, .99999994f);
                rigidBody.m_motion.m_motionState.m_sweptTransform_4 = new Vector4(.0f);
                rigidBody.m_motion.m_motionState.m_deltaAngle = new Vector4(.0f);
                rigidBody.m_motion.m_motionState.m_objectRadius = 2.25f;
                rigidBody.m_motion.m_motionState.m_linearDamping = 0;
                rigidBody.m_motion.m_motionState.m_angularDamping = 0x3D4D;
                rigidBody.m_motion.m_motionState.m_timeFactor = 0x3F80;
                rigidBody.m_motion.m_motionState.m_maxLinearVelocity = new hkUFloat8();
                rigidBody.m_motion.m_motionState.m_maxLinearVelocity.m_value = 127;
                rigidBody.m_motion.m_motionState.m_maxAngularVelocity = new hkUFloat8();
                rigidBody.m_motion.m_motionState.m_maxAngularVelocity.m_value = 127;
                rigidBody.m_motion.m_motionState.m_deactivationClass = 1;
                rigidBody.m_motion.m_inertiaAndMassInv = new Vector4(.0f);
                rigidBody.m_motion.m_linearVelocity = new Vector4(.0f);
                rigidBody.m_motion.m_angularVelocity = new Vector4(.0f);
                rigidBody.m_motion.m_deactivationRefPosition_0 = new Vector4(.0f);
                rigidBody.m_motion.m_deactivationRefPosition_1 = new Vector4(.0f);
                rigidBody.m_motion.m_deactivationRefOrientation_0 = 0;
                rigidBody.m_motion.m_deactivationRefOrientation_1 = 0;
                rigidBody.m_motion.m_savedMotion = null;
                rigidBody.m_motion.m_savedQualityTypeIndex = 0;
                rigidBody.m_motion.m_gravityFactor = 0x3F80;
                rigidBody.m_localFrame = null;
                rigidBody.m_npData = 0xFFFFFFFF;
                system.m_rigidBodies.Add(rigidBody);
                #endregion
                
                #region hkpBvCompressedMeshShape
                var cmesh = hkpBvCompressedMeshShapeBuilder.Build(verts, indices);
                rigidBody.m_collidable.m_shape = cmesh;
                #endregion

                using (FileStream fs = File.Create(args[0] + ".hkrb"))
                {
                    s.Serialize(root, new BinaryWriterEx(fs), HKXHeader.BotwWiiu());
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}