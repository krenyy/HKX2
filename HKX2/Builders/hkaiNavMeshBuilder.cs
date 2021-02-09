﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HKX2.Builders
{
    public static class hkaiNavMeshBuilder
    {
        public struct BuildParams
        {
            public float Cellsize;
            public float Cellheight;
            public float SlopeAngle;
            public float AgentHeight;
            public float AgentClimb;
            public float AgentRadius;
            public int MinRegionArea;

            public static BuildParams DefaultParams()
            {
                var ret = new BuildParams();
                ret.Cellsize = 0.3f;
                ret.Cellheight = 0.3f;
                ret.SlopeAngle = 30.0f;
                ret.AgentHeight = 2.0f;
                ret.AgentClimb = 0.5f;
                ret.AgentRadius = 0.5f;
                ret.MinRegionArea = 3;
                return ret;
            }
        }

        public static hkRootLevelContainer BuildNavmesh(BuildParams p, List<Vector3> verts, List<int> indices)
        {
            var root = new hkRootLevelContainer();
            NavMeshNative.SetNavmeshBuildParams(p.Cellsize, p.Cellheight, p.SlopeAngle, p.AgentHeight, p.AgentClimb, p.AgentRadius, p.MinRegionArea);
            var buildSuccess = NavMeshNative.BuildNavmeshForMesh(verts.ToArray(), verts.Count, indices.ToArray(), indices.Count);
            if (!buildSuccess)
            {
                return null;
            }

            var vcount = NavMeshNative.GetMeshVertCount();
            var icount = NavMeshNative.GetMeshTriCount();
            if (vcount == 0 || icount == 0)
            {
                return null;
            }

            ushort[] bverts = new ushort[vcount * 3];
            ushort[] bindices = new ushort[icount * 3 * 2];
            Vector3[] vbverts = new Vector3[vcount];
            NavMeshNative.GetMeshVerts(bverts);
            NavMeshNative.GetMeshTris(bindices);

            Vector3[] bounds = new Vector3[2];
            NavMeshNative.GetBoundingBox(bounds);

            var nmesh = new hkaiNavMesh();
            nmesh.m_aabb = new hkAabb();
            nmesh.m_aabb.m_min = new Vector4(bounds[0].X, bounds[0].Y, bounds[0].Z, 1.0f);
            nmesh.m_aabb.m_max = new Vector4(bounds[1].X, bounds[1].Y, bounds[1].Z, 1.0f);

            nmesh.m_edgeData = new List<int>();
            nmesh.m_edgeDataStriding = 1;
            nmesh.m_edges = new List<hkaiNavMeshEdge>();
            nmesh.m_erosionRadius = 0.0f;
            nmesh.m_faceData = new List<int>();
            nmesh.m_faceDataStriding = 1;
            nmesh.m_faces = new List<hkaiNavMeshFace>();
            nmesh.m_flags = 0;
            nmesh.m_vertices = new List<Vector4>();

            for (int i = 0; i < bverts.Length/3; i++)
            {
                var vx = bverts[i * 3];
                var vy = bverts[i * 3 + 1];
                var vz = bverts[i * 3 + 2];

                var vert = new Vector3(bounds[0].X + vx * p.Cellsize,
                                       bounds[0].Y + vy * p.Cellheight,
                                       bounds[0].Z + vz * p.Cellsize);
                nmesh.m_vertices.Add(new Vector4(vert.X, vert.Y, vert.Z, 1.0f));
                vbverts[i] = vert;
            }

            for (int t = 0; t < bindices.Length/2; t += 3)
            {
                var f = new hkaiNavMeshFace();
                f.m_clusterIndex = 0;
                f.m_numEdges = 3;
                f.m_startEdgeIndex = nmesh.m_edges.Count;
                f.m_startUserEdgeIndex = -1;
                f.m_padding = 0xCDCD;
                nmesh.m_faces.Add(f);
                nmesh.m_faceData.Add(0);

                for (int i = 0; i < 3; i++)
                {
                    var e = new hkaiNavMeshEdge();
                    e.m_a = bindices[t * 2 + i];
                    e.m_b = bindices[t * 2 + ((i + 1) % 3)];
                    e.m_flags = EdgeFlagBits.EDGE_ORIGINAL;
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
                        for (int j = 0; j < 3; j++)
                        {
                            var edge = bindices[t * 2 + 3 + i] * 6 + 3 + j;
                            if (bindices[edge] == (t / 3))
                            {
                                e.m_oppositeEdge = (uint)bindices[t * 2 + 3 + i] * 3 + (uint)j;
                            }
                        }
                    }
                    nmesh.m_edges.Add(e);
                    nmesh.m_edgeData.Add(0);
                }
            }

            var namedVariant01 = new hkRootLevelContainerNamedVariant();
            namedVariant01.m_className = "hkaiNavMesh";
            namedVariant01.m_name = "00/000,+0000,+0000,+0000//NavMesh";
            namedVariant01.m_variant = nmesh;

            // Next step: build a bvh
            var shortIndices = new ushort[bindices.Length / 2];
            for (int i = 0; i < bindices.Length / 2; i += 3)
            {
                shortIndices[i] = bindices[i * 2];
                shortIndices[i + 1] = bindices[i * 2 + 1];
                shortIndices[i + 2] = bindices[i * 2 + 2];
            }
            bool didbuild = BVHNative.BuildBVHForMesh(vbverts, vbverts.Length, shortIndices.Select(arg => (uint) arg).ToArray(), shortIndices.Length);
            if (!didbuild)
            {
                return null;
            }

            var nodecount = BVHNative.GetBVHSize();
            var nsize = BVHNative.GetNodeSize();
            var nodes = new NativeBVHNode[nodecount];
            BVHNative.GetBVHNodes(nodes);

            // Rebuild in friendlier tree form
            List<BVNode> bnodes = new List<BVNode>((int)nodecount);
            foreach (var n in nodes)
            {
                var bnode = new BVNode();
                bnode.Min = new Vector3(n.minX, n.minY, n.minZ);
                bnode.Max = new Vector3(n.maxX, n.maxY, n.maxZ);
                bnode.IsLeaf = n.isLeaf;
                bnode.PrimitiveCount = n.primitiveCount;
                bnode.Primitive = n.firstChildOrPrimitive;
                bnodes.Add(bnode);
            }
            for (int i = 0; i < nodes.Length; i++)
            {
                if (!nodes[i].isLeaf)
                {
                    bnodes[i].Left = bnodes[(int)nodes[i].firstChildOrPrimitive];
                    bnodes[i].Right = bnodes[(int)nodes[i].firstChildOrPrimitive + 1];
                }
            }
            
            var tree = new hkcdStaticAabbTree();
            tree.m_treePtr = new hkcdStaticTreeDefaultTreeStorage6();
            tree.m_treePtr.m_nodes = bnodes[0].BuildAxis6Tree();
            var min = bnodes[0].Min;
            var max = bnodes[0].Max;
            tree.m_treePtr.m_domain = new hkAabb();
            tree.m_treePtr.m_domain.m_min = new Vector4(min.X, min.Y, min.Z, 1.0f);
            tree.m_treePtr.m_domain.m_max = new Vector4(max.X, max.Y, max.Z, 1.0f);

            // Build a dummy directed graph
            var namedVariant02 = new hkRootLevelContainerNamedVariant();
            namedVariant02.m_className = "hkaiDirectedGraphExplicitCost";
            namedVariant02.m_name = "00/000,+0000,+0000,+0000//ClusterGraph";
            var graph = new hkaiDirectedGraphExplicitCost();
            namedVariant02.m_variant = graph;

            graph.m_nodes = new List<hkaiDirectedGraphExplicitCostNode>();
            var node = new hkaiDirectedGraphExplicitCostNode();
            node.m_numEdges = 0;
            node.m_startEdgeIndex = 0;
            graph.m_nodes.Add(node);

            graph.m_positions = new List<Vector4>();
            var c = (max - min) / 2;
            graph.m_positions.Add(new Vector4(c.X, c.Y, c.Z, 1.0f));

            // Query Mediator
            var namedVariant03 = new hkRootLevelContainerNamedVariant();
            namedVariant03.m_className = "hkaiStaticTreeNavMeshQueryMediator";
            namedVariant03.m_name = "00/000,+0000,+0000,+0000//QueryMediator";
            var queryMediator = new hkaiStaticTreeNavMeshQueryMediator();
            queryMediator.m_tree = tree;
            queryMediator.m_navMesh = nmesh;
            namedVariant03.m_variant = queryMediator;

            root.m_namedVariants = new List<hkRootLevelContainerNamedVariant>();
            root.m_namedVariants.Add(namedVariant01);
            root.m_namedVariants.Add(namedVariant02);
            root.m_namedVariants.Add(namedVariant03);
            
            return root;
        }
    }
}
