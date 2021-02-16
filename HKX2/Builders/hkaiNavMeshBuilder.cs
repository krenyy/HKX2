using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HKX2.Builders
{
    public static class hkaiNavMeshBuilder
    {
        public struct BuildParams
        {
            public float CellSize;
            public float CellHeight;
            public float WalkableSlopeAngle;
            public float WalkableHeight;
            public float WalkableClimb;
            public float WalkableRadius;
            public int MinRegionArea;

            public static BuildParams Default()
            {
                return new BuildParams
                {
                    CellSize = 0.3f,
                    CellHeight = 0.3f,
                    WalkableSlopeAngle = 30.0f,
                    WalkableHeight = 2.0f,
                    WalkableClimb = 1.0f,
                    WalkableRadius = 0.5f,
                    MinRegionArea = 3
                };
            }
        }

        public static hkRootLevelContainer BuildNavmesh(BuildParams p, List<Vector3> verts, List<int> indices)
        {
            var root = new hkRootLevelContainer();
            NavMeshNative.SetNavmeshBuildParams(
                p.CellSize, p.CellHeight,
                p.WalkableSlopeAngle, p.WalkableHeight,
                p.WalkableClimb, p.WalkableRadius,
                p.MinRegionArea);
            var buildSuccess = NavMeshNative.BuildNavmeshForMesh(
                verts.ToArray(), verts.Count, indices.ToArray(), indices.Count);
            if (!buildSuccess)
            {
                throw new Exception("Couldn't build Navmesh!");
            }

            var vcount = NavMeshNative.GetMeshVertCount();
            var icount = NavMeshNative.GetMeshTriCount();
            if (vcount == 0 || icount == 0)
            {
                throw new Exception("Resulting Navmesh is empty!");
            }

            var bverts = new ushort[vcount * 3];
            var bindices = new ushort[icount * 3 * 2];
            var vbverts = new Vector3[vcount];
            NavMeshNative.GetMeshVerts(bverts);
            NavMeshNative.GetMeshTris(bindices);

            var bounds = new Vector3[2];
            NavMeshNative.GetBoundingBox(bounds);

            var nmesh = new hkaiNavMesh
            {
                m_faces = new List<hkaiNavMeshFace>(),
                m_edges = new List<hkaiNavMeshEdge>(),
                m_vertices = new List<Vector4>(),
                m_streamingSets = new List<hkaiStreamingSet>(),
                m_faceData = new List<int>(),
                m_edgeData = new List<int>(),
                m_faceDataStriding = 1,
                m_edgeDataStriding = 1,
                m_flags = NavMeshFlagBits.MESH_NONE,
                m_aabb = new hkAabb
                {
                    m_min = new Vector4(bounds[0].X, bounds[0].Y, bounds[0].Z, 1.0f),
                    m_max = new Vector4(bounds[1].X, bounds[1].Y, bounds[1].Z, 1.0f)
                },
                m_erosionRadius = 0.0f,
                m_userData = 0
            };

            for (var i = 0; i < bverts.Length / 3; i++)
            {
                var vx = bverts[i * 3];
                var vy = bverts[i * 3 + 1];
                var vz = bverts[i * 3 + 2];

                var vert = new Vector3(bounds[0].X + vx * p.CellSize,
                    bounds[0].Y + vy * p.CellHeight,
                    bounds[0].Z + vz * p.CellSize);
                nmesh.m_vertices.Add(new Vector4(vert.X, vert.Y, vert.Z, 1.0f));
                vbverts[i] = vert;
            }

            for (var t = 0; t < bindices.Length / 2; t += 3)
            {
                nmesh.m_faces.Add(
                    new hkaiNavMeshFace
                    {
                        m_clusterIndex = 0,
                        m_numEdges = 3,
                        m_startEdgeIndex = nmesh.m_edges.Count,
                        m_startUserEdgeIndex = -1,
                        m_padding = 0xCDCD
                    });
                nmesh.m_faceData.Add(0);

                for (var i = 0; i < 3; i++)
                {
                    var e = new hkaiNavMeshEdge
                    {
                        m_a = bindices[t * 2 + i],
                        m_b = bindices[t * 2 + ((i + 1) % 3)],
                        m_flags = EdgeFlagBits.EDGE_ORIGINAL
                    };
                    // Record adjacency
                    if (bindices[t * 2 + 3 + i] == 0xFFFF)
                    {
                        // No adjacency
                        e.m_oppositeEdge = 0xFFFFFFFF;
                        e.m_oppositeFace = 0xFFFFFFFF;
                    }
                    else
                    {
                        e.m_oppositeFace = bindices[t * 2 + 3 + i];
                        // Find the edge that has this face as an adjacency
                        for (var j = 0; j < 3; j++)
                        {
                            var edge = bindices[t * 2 + 3 + i] * 6 + 3 + j;
                            if (bindices[edge] == (t / 3))
                            {
                                e.m_oppositeEdge = (uint) bindices[t * 2 + 3 + i] * 3 + (uint) j;
                            }
                        }
                    }

                    nmesh.m_edges.Add(e);
                    nmesh.m_edgeData.Add(0);
                }
            }

            var namedVariant01 = new hkRootLevelContainerNamedVariant
            {
                m_className = "hkaiNavMesh",
                m_name = "00/000,+0000,+0000,+0000//NavMesh",
                m_variant = nmesh
            };

            // Next step: build a bvh
            var shortIndices = new ushort[bindices.Length / 2];
            for (var i = 0; i < bindices.Length / 2; i += 3)
            {
                shortIndices[i] = bindices[i * 2];
                shortIndices[i + 1] = bindices[i * 2 + 1];
                shortIndices[i + 2] = bindices[i * 2 + 2];
            }

            var didbuild = BVHNative.BuildBVHForMesh(vbverts, shortIndices.Select(arg => (uint) arg).ToArray(),
                shortIndices.Length);
            if (!didbuild)
            {
                return null;
            }

            var nodecount = BVHNative.GetBVHSize();
            var nodes = new NativeBVHNode[nodecount];
            BVHNative.GetBVHNodes(nodes);

            // Rebuild in friendlier tree form
            var bnodes = nodes.Select(n => new BVNode
            {
                Min = new Vector3(n.minX, n.minY, n.minZ),
                Max = new Vector3(n.maxX, n.maxY, n.maxZ),
                IsLeaf = n.isLeaf,
                PrimitiveCount = n.primitiveCount,
                Primitive = n.firstChildOrPrimitive
            }).ToList();

            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].isLeaf) continue;
                bnodes[i].Left = bnodes[(int) nodes[i].firstChildOrPrimitive];
                bnodes[i].Right = bnodes[(int) nodes[i].firstChildOrPrimitive + 1];
            }

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

            // Build a dummy directed graph
            var c = (max - min) / 2;
            var namedVariant02 = new hkRootLevelContainerNamedVariant
            {
                m_className = "hkaiDirectedGraphExplicitCost",
                m_name = "00/000,+0000,+0000,+0000//ClusterGraph",
                m_variant = new hkaiDirectedGraphExplicitCost
                {
                    m_positions = new List<Vector4> {new Vector4(c.X, c.Y, c.Z, 1.0f)},
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
                }
            };

            // Query Mediator
            var namedVariant03 = new hkRootLevelContainerNamedVariant
            {
                m_className = "hkaiStaticTreeNavMeshQueryMediator",
                m_name = "00/000,+0000,+0000,+0000//QueryMediator",
                m_variant = new hkaiStaticTreeNavMeshQueryMediator {m_tree = tree, m_navMesh = nmesh}
            };

            root.m_namedVariants = new List<hkRootLevelContainerNamedVariant>
            {
                namedVariant01, namedVariant02, namedVariant03
            };

            return root;
        }
    }
}