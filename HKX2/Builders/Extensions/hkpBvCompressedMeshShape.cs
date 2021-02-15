using System;
using System.Numerics;

namespace HKX2.Builders.Extensions
{
    // Represents a tree node for a mesh's BVH tree when it's expanded from its packed format
    [Serializable]
    public class BVHNode
    {
        // Bounding box AABB that contains all the children as well
        public Vector3 Min;
        public Vector3 Max;

        // Left and right children nodes
        public BVHNode Left;
        public BVHNode Right;

        // Terminal leaf in the node which means it points directly to a chunk or a triangle
        public bool IsTerminal;

        // If a terminal, this is the index of the chunk/triangle for this terminal
        public uint Index;
    }

    public static partial class Extensions
    {
        private static BVHNode BuildBvhTree(this hkpBvCompressedMeshShape _this, Vector3 parentBBMin,
            Vector3 parentBBMax, uint nodeIndex)
        {
            BVHNode node = new BVHNode();
            var cnode = _this.m_tree.m_nodes[(int) nodeIndex];
            node.Min = cnode.DecompressMin(parentBBMin, parentBBMax);
            node.Max = cnode.DecompressMax(parentBBMin, parentBBMax);

            if ((cnode.m_hiData & 0x80) > 0)
            {
                node.Left = BuildBvhTree(_this, node.Min, node.Max, nodeIndex + 1);
                node.Right = BuildBvhTree(_this, node.Min, node.Max,
                    nodeIndex + ((((uint) cnode.m_hiData & 0x7F) << 8) | cnode.m_loData) * 2);
            }
            else
            {
                node.IsTerminal = true;
                node.Index = (((uint) cnode.m_hiData & 0x7F) << 8) | cnode.m_loData;
            }

            return node;
        }

        // Extracts an easily processable BVH tree from the packed version in the mesh data
        public static BVHNode GetMeshBvh(this hkpBvCompressedMeshShape _this)
        {
            if (_this.m_tree.m_nodes == null || _this.m_tree.m_nodes.Count == 0)
            {
                return null;
            }

            BVHNode root = new BVHNode();
            root.Min = new Vector3(_this.m_tree.m_domain.m_min.X, _this.m_tree.m_domain.m_min.Y,
                _this.m_tree.m_domain.m_min.Z);
            root.Max = new Vector3(_this.m_tree.m_domain.m_max.X, _this.m_tree.m_domain.m_max.Y,
                _this.m_tree.m_domain.m_max.Z);

            var cnode = _this.m_tree.m_nodes[0];
            if ((cnode.m_hiData & 0x80) > 0)
            {
                root.Left = BuildBvhTree(_this, root.Min, root.Max, 1);
                root.Right = BuildBvhTree(_this, root.Min, root.Max,
                    ((((uint) cnode.m_hiData & 0x7F) << 8) | cnode.m_loData) * 2);
            }
            else
            {
                root.IsTerminal = true;
                root.Index = (((uint) cnode.m_hiData & 0x7F) << 8) | cnode.m_loData;
            }

            return root;
        }
    }
}