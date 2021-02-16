using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
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
            using (var fs = File.OpenRead(args[0]))
            {
                var fact = new ObjLoaderFactory();
                var loader = fact.Create();
                res = loader.Load(fs);
            }

            var indices = new List<uint>();
            var primitiveInfos = new List<Tuple<uint, uint>>();

            var verts = res.Vertices.Select(vert => new Vector3(vert.X, vert.Y, vert.Z)).ToList();

            foreach (var group in res.Groups)
            {
                var match = Regex.Match(group.Name, @"\[([a-zA-Z0-9]+)\]");
                var primitiveInfo = Convert.ToUInt64(
                    match.Value == "" ? "9000000000000002" : match.Groups[1].Value, 16);

                foreach (var idx in group.Faces)
                {
                    indices.Add((uint) (idx[0].VertexIndex - 1));
                    indices.Add((uint) (idx[1].VertexIndex - 1));
                    indices.Add((uint) (idx[2].VertexIndex - 1));
                    primitiveInfos.Add(
                        new Tuple<uint, uint>(
                            (uint) (primitiveInfo >> 32),
                            (uint) primitiveInfo & 0xFFFFFFFF
                        )
                    );
                }
            }

            var extension = args[1];
            var roots = new List<IHavokObject>();
            switch (extension)
            {
                case "hksc":
                {
                    var staticCompoundShape = new hkpStaticCompoundShape
                    {
                        m_dispatchType = ShapeDispatchTypeEnum.USER,
                        m_bitsPerKey = 0,
                        m_shapeInfoCodecType = ShapeInfoCodecTypeEnum.NULL_CODEC,
                        m_userData = 0,
                        m_bvTreeType = BvTreeType.BVTREE_STATIC_COMPOUND,
                        m_numBitsForChildShapeKey = 32,
                        m_instances = new List<hkpStaticCompoundShapeInstance>
                        {
                            new hkpStaticCompoundShapeInstance
                            {
                                m_transform = new Matrix4x4(
                                    0f, 0f, 0f, .5000001f,
                                    0f, 0f, 0f, 1f,
                                    1f, 1f, 1f, .5f,
                                    0f, 0f, 0f, 1f),
                                m_shape = null,
                                m_filterInfo = 0,
                                m_childFilterInfoMask = 0xFFFFFFFF,
                                m_userData = 0 // ShapeInfo index
                            }
                        },
                        m_instanceExtraInfos = new List<ushort>(),
                        m_disabledLargeShapeKeyTable = new hkpShapeKeyTable {m_lists = null, m_occupancyBitField = 0},
                        m_tree = new hkcdStaticTreeDefaultTreeStorage6
                        {
                            m_domain = new hkAabb(),
                            m_nodes = new List<hkcdStaticTreeCodec3Axis6>
                            {
                                new hkcdStaticTreeCodec3Axis6
                                {
                                    m_xyz_0 = 0,
                                    m_xyz_1 = 0,
                                    m_xyz_2 = 0,
                                    m_hiData = 0,
                                    m_loData = 0
                                }
                            }
                        }
                    };

                    var compressedMeshShape = hkpBvCompressedMeshShapeBuilder.Build(verts, indices, primitiveInfos);
                    staticCompoundShape.m_instances[0].m_shape = compressedMeshShape;

                    var cmeshDomain = compressedMeshShape.m_tree.m_domain;
                    var min = cmeshDomain.m_min;
                    var max = cmeshDomain.m_max;
                    staticCompoundShape.m_tree.m_domain.m_min = new Vector4(min.X, min.Y, min.Z, 0f);
                    staticCompoundShape.m_tree.m_domain.m_max = new Vector4(max.X, max.Y, max.Z, 0f);

                    // Really important, otherwise CTD
                    staticCompoundShape.m_numBitsForChildShapeKey = (sbyte) compressedMeshShape.m_tree.m_bitsPerKey;

                    // For some reason, there are always 17 systems, only one of them is used
                    var systems = (from i in Enumerable.Range(0, 17)
                        select new hkpPhysicsSystem
                        {
                            m_rigidBodies = new List<hkpRigidBody>(),
                            m_constraints = new List<hkpConstraintInstance>(),
                            m_actions = new List<hkpAction>(),
                            m_phantoms = new List<hkpPhantom>(),
                            m_name = $@"Compound_{i}",
                            m_userData = 0,
                            m_active = true
                        }).ToList();

                    systems[0].m_rigidBodies.Add(new hkpRigidBody
                    {
                        m_userData = 0,
                        m_collidable =
                            new hkpLinkedCollidable
                            {
                                m_shape = staticCompoundShape,
                                m_shapeKey = 0xFFFFFFFF,
                                m_forceCollideOntoPpu = 8,
                                m_broadPhaseHandle = new hkpTypedBroadPhaseHandle
                                {
                                    m_type = BroadPhaseType.BROAD_PHASE_ENTITY,
                                    m_objectQualityType = 0,
                                    m_collisionFilterInfo = 0x3C000088
                                },
                                m_allowedPenetrationDepth = float.MaxValue
                            },
                        m_multiThreadCheck = new hkMultiThreadCheck(),
                        m_name = "Compound_EntityGround_0",
                        m_properties = new List<hkSimpleProperty>(),
                        m_material =
                            new hkpMaterial
                            {
                                m_responseType = ResponseType.RESPONSE_SIMPLE_CONTACT,
                                m_rollingFrictionMultiplier = 0,
                                m_friction = .5f,
                                m_restitution = .4f
                            },
                        m_damageMultiplier = 1f,
                        m_storageIndex = 0xFFFF,
                        m_contactPointCallbackDelay = 0xFFFF,
                        m_autoRemoveLevel = 0,
                        m_numShapeKeysInContactPointProperties = 2,
                        m_responseModifierFlags = 0,
                        m_uid = 0xFFFFFFFF,
                        m_spuCollisionCallback =
                            new hkpEntitySpuCollisionCallback
                            {
                                m_eventFilter =
                                    SpuCollisionCallbackEventFilter.SPU_SEND_CONTACT_POINT_ADDED_OR_PROCESS,
                                m_userFilter = 1
                            },
                        m_motion = new hkpMaxSizeMotion
                        {
                            m_type = MotionType.MOTION_FIXED,
                            m_deactivationIntegrateCounter = 15,
                            m_deactivationNumInactiveFrames_0 = 0xC000,
                            m_deactivationNumInactiveFrames_1 = 0xC000,
                            m_motionState =
                                new hkMotionState
                                {
                                    m_transform = Matrix4x4.Identity,
                                    m_sweptTransform_0 = new Vector4(0f),
                                    m_sweptTransform_1 = new Vector4(0f),
                                    m_sweptTransform_2 = new Vector4(0f, 0f, 0f, 1f),
                                    m_sweptTransform_3 = new Vector4(0f, 0f, 0f, 1f),
                                    m_sweptTransform_4 = new Vector4(0f),
                                    m_deltaAngle = new Vector4(0f),
                                    m_objectRadius = 8f,
                                    m_linearDamping = 0,
                                    m_angularDamping = 0x3D4D,
                                    m_timeFactor = 0x3F80,
                                    m_maxLinearVelocity = new hkUFloat8 {m_value = 127},
                                    m_maxAngularVelocity = new hkUFloat8 {m_value = 127},
                                    m_deactivationClass = 1
                                },
                            m_inertiaAndMassInv = new Vector4(0f),
                            m_linearVelocity = new Vector4(0f),
                            m_angularVelocity = new Vector4(0f),
                            m_deactivationRefPosition_0 = new Vector4(0f),
                            m_deactivationRefPosition_1 = new Vector4(0f),
                            m_deactivationRefOrientation_0 = 0,
                            m_deactivationRefOrientation_1 = 0,
                            m_savedMotion = null,
                            m_savedQualityTypeIndex = 0,
                            m_gravityFactor = 0x3F80
                        },
                        m_localFrame = null,
                        m_npData = 0xFFFFFFFF
                    });

                    roots.Add(new StaticCompoundInfo
                    {
                        m_Offset = 0,
                        m_ActorInfo =
                            new List<ActorInfo>
                            {
                                new ActorInfo
                                {
                                    m_HashId = 0, m_SRTHash = 0, m_ShapeInfoStart = 0, m_ShapeInfoEnd = 0
                                }
                            },
                        m_ShapeInfo = new List<ShapeInfo>
                        {
                            new ShapeInfo
                            {
                                m_ActorInfoIndex = 0, m_InstanceId = 0, m_BodyGroup = 0, m_BodyLayerType = 0
                            }
                        }
                    });
                    roots.Add(new hkRootLevelContainer
                    {
                        m_namedVariants = new List<hkRootLevelContainerNamedVariant>
                        {
                            new hkRootLevelContainerNamedVariant
                            {
                                m_name = "hkpPhysicsData",
                                m_className = "hkpPhysicsData",
                                m_variant = new hkpPhysicsData
                                {
                                    m_worldCinfo = null,
                                    m_systems = systems
                                }
                            }
                        }
                    });

                    break;
                }
                case "hkrb":
                {
                    roots.Add(new hkRootLevelContainer
                    {
                        m_namedVariants = new List<hkRootLevelContainerNamedVariant>
                        {
                            new hkRootLevelContainerNamedVariant
                            {
                                m_name = "Physics Data",
                                m_className = "hkpPhysicsData",
                                m_variant = new hkpPhysicsData
                                {
                                    m_worldCinfo = null,
                                    m_systems = new List<hkpPhysicsSystem>
                                    {
                                        new hkpPhysicsSystem
                                        {
                                            m_rigidBodies = new List<hkpRigidBody>
                                            {
                                                new hkpRigidBody
                                                {
                                                    m_userData = 0,
                                                    m_collidable =
                                                        new hkpLinkedCollidable
                                                        {
                                                            m_shape = hkpBvCompressedMeshShapeBuilder.Build(verts,
                                                                indices, primitiveInfos),
                                                            m_shapeKey = 0xFFFFFFFF,
                                                            m_forceCollideOntoPpu = 8,
                                                            m_broadPhaseHandle = new hkpTypedBroadPhaseHandle
                                                            {
                                                                m_type = BroadPhaseType.BROAD_PHASE_ENTITY,
                                                                m_objectQualityType = 0,
                                                                m_collisionFilterInfo = 0x90000000
                                                            },
                                                            m_allowedPenetrationDepth = float.MaxValue
                                                        },
                                                    m_multiThreadCheck = new hkMultiThreadCheck(),
                                                    m_name = "Collision_IDK",
                                                    m_properties = new List<hkSimpleProperty>(),
                                                    m_material =
                                                        new hkpMaterial
                                                        {
                                                            m_responseType = ResponseType.RESPONSE_SIMPLE_CONTACT,
                                                            m_rollingFrictionMultiplier = 0,
                                                            m_friction = .5f,
                                                            m_restitution = .4f
                                                        },
                                                    m_damageMultiplier = 1f,
                                                    m_storageIndex = 0xFFFF,
                                                    m_contactPointCallbackDelay = 0xFFFF,
                                                    m_autoRemoveLevel = 0,
                                                    m_numShapeKeysInContactPointProperties = 1,
                                                    m_responseModifierFlags = 0,
                                                    m_uid = 0xFFFFFFFF,
                                                    m_spuCollisionCallback =
                                                        new hkpEntitySpuCollisionCallback
                                                        {
                                                            m_eventFilter =
                                                                SpuCollisionCallbackEventFilter
                                                                    .SPU_SEND_CONTACT_POINT_ADDED_OR_PROCESS,
                                                            m_userFilter = 1
                                                        },
                                                    m_motion = new hkpMaxSizeMotion
                                                    {
                                                        m_type = MotionType.MOTION_FIXED,
                                                        m_deactivationIntegrateCounter = 15,
                                                        m_deactivationNumInactiveFrames_0 = 0xC000,
                                                        m_deactivationNumInactiveFrames_1 = 0xC000,
                                                        m_motionState =
                                                            new hkMotionState
                                                            {
                                                                m_transform = Matrix4x4.Identity,
                                                                m_sweptTransform_0 = new Vector4(.0f),
                                                                m_sweptTransform_1 = new Vector4(.0f),
                                                                m_sweptTransform_2 =
                                                                    new Vector4(.0f, .0f, .0f, .99999994f),
                                                                m_sweptTransform_3 =
                                                                    new Vector4(.0f, .0f, .0f, .99999994f),
                                                                m_sweptTransform_4 = new Vector4(.0f),
                                                                m_deltaAngle = new Vector4(.0f),
                                                                m_objectRadius = 2.25f,
                                                                m_linearDamping = 0,
                                                                m_angularDamping = 0x3D4D,
                                                                m_timeFactor = 0x3F80,
                                                                m_maxLinearVelocity = new hkUFloat8 {m_value = 127},
                                                                m_maxAngularVelocity = new hkUFloat8 {m_value = 127},
                                                                m_deactivationClass = 1
                                                            },
                                                        m_inertiaAndMassInv = new Vector4(.0f),
                                                        m_linearVelocity = new Vector4(.0f),
                                                        m_angularVelocity = new Vector4(.0f),
                                                        m_deactivationRefPosition_0 = new Vector4(.0f),
                                                        m_deactivationRefPosition_1 = new Vector4(.0f),
                                                        m_deactivationRefOrientation_0 = 0,
                                                        m_deactivationRefOrientation_1 = 0,
                                                        m_savedMotion = null,
                                                        m_savedQualityTypeIndex = 0,
                                                        m_gravityFactor = 0x3F80
                                                    },
                                                    m_localFrame = null,
                                                    m_npData = 0xFFFFFFFF
                                                }
                                            },
                                            m_constraints = new List<hkpConstraintInstance>(),
                                            m_actions = new List<hkpAction>(),
                                            m_phantoms = new List<hkpPhantom>(),
                                            m_name = "Default Physics System",
                                            m_userData = 0,
                                            m_active = true
                                        }
                                    }
                                }
                            }
                        }
                    });

                    break;
                }
                case "hknm2":
                {
                    var c = hkaiNavMeshBuilder.Config.Default();
                    var navMesh = hkaiNavMeshBuilder.Build(c, verts, indices.Select(i => (int) i).ToList());

                    var vbverts = navMesh.m_vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                    var bindices = new ushort[NavMeshNative.GetMeshTriCount() * 3 * 2];
                    NavMeshNative.GetMeshTris(bindices);

                    // Build BVH
                    var shortIndices = new ushort[bindices.Length / 2];
                    for (var i = 0; i < bindices.Length / 2; i += 3)
                    {
                        shortIndices[i] = bindices[i * 2];
                        shortIndices[i + 1] = bindices[i * 2 + 1];
                        shortIndices[i + 2] = bindices[i * 2 + 2];
                    }

                    var bnodes = BVNode.BuildBVHForMesh(
                        vbverts,
                        shortIndices.Select(i => (uint) i).ToArray(),
                        shortIndices.Length);

                    var min = bnodes[0].Min;
                    var max = bnodes[0].Max;
                    var tree = new hkcdStaticAabbTree
                    {
                        m_treePtr = new hkcdStaticTreeDefaultTreeStorage6
                        {
                            m_nodes = bnodes[0].BuildAxis6Tree(),
                            m_domain = new hkAabb
                            {
                                m_min = new Vector4(min.X, min.Y, min.Z, 1.0f),
                                m_max = new Vector4(max.X, max.Y, max.Z, 1.0f)
                            }
                        }
                    };

                    var domain = tree.m_treePtr.m_domain;
                    var center = (domain.m_max - domain.m_min) / 2;
                    var costGraph = new hkaiDirectedGraphExplicitCost
                    {
                        m_positions = new List<Vector4>
                        {
                            new Vector4(center.X, center.Y, center.Z, 1.0f)
                        },
                        m_nodes = new List<hkaiDirectedGraphExplicitCostNode>
                        {
                            new hkaiDirectedGraphExplicitCostNode {m_numEdges = 0, m_startEdgeIndex = 0}
                        },
                        m_edges = new List<hkaiDirectedGraphExplicitCostEdge>(),
                        m_nodeData = new List<uint>(),
                        m_edgeData = new List<uint>(),
                        m_nodeDataStriding = 0,
                        m_edgeDataStriding = 0,
                        m_streamingSets = new List<hkaiStreamingSet>()
                    };;

                    roots.Add(new hkRootLevelContainer
                    {
                        m_namedVariants = new List<hkRootLevelContainerNamedVariant>
                        {
                            new hkRootLevelContainerNamedVariant
                            {
                                m_className = "hkaiNavMesh",
                                m_name = "00/000,+0000,+0000,+0000//NavMesh",
                                m_variant = navMesh
                            },
                            new hkRootLevelContainerNamedVariant
                            {
                                m_className = "hkaiDirectedGraphExplicitCost",
                                m_name = "00/000,+0000,+0000,+0000//ClusterGraph",
                                m_variant = costGraph
                            },
                            new hkRootLevelContainerNamedVariant
                            {
                                m_className = "hkaiStaticTreeNavMeshQueryMediator",
                                m_name = "00/000,+0000,+0000,+0000//QueryMediator",
                                m_variant = new hkaiStaticTreeNavMeshQueryMediator
                                {
                                    m_tree = tree,
                                    m_navMesh = navMesh
                                }
                            }
                        }
                    });

                    break;
                }
                default: throw new Exception();
            }

            using (var ws = File.OpenWrite($@"{args[0]}.{args[1]}"))
            {
                Util.WriteBotwHKX(roots, HKXHeader.BotwWiiu(), extension, ws);
            }
        }
    }
}