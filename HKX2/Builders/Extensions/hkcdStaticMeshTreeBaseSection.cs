﻿using System.Numerics;

namespace HKX2.Builders.Extensions
{
    public static partial class Extensions
    {
        // Recursively builds the BVH tree from the compressed packed array
        public static BVHNode buildBVHTree(this hkcdStaticMeshTreeBaseSection _this, Vector3 parentBBMin, Vector3 parentBBMax, uint nodeIndex)
        {
            BVHNode node = new BVHNode();
            var cnode = _this.m_nodes[(int)nodeIndex];
            node.Min = cnode.DecompressMin(parentBBMin, parentBBMax);
            node.Max = cnode.DecompressMax(parentBBMin, parentBBMax);

            if ((cnode.m_data & 0x01) > 0)
            {
                node.Left = buildBVHTree(_this, node.Min, node.Max, nodeIndex + 1);
                node.Right = buildBVHTree(_this, node.Min, node.Max, nodeIndex + ((uint)cnode.m_data & 0xFE));
            }
            else
            {
                node.IsTerminal = true;
                node.Index = (uint)cnode.m_data / 2;
            }
            return node;
        }

        // Extracts an easily processable BVH tree from the packed version in the mesh data
        public static BVHNode getSectionBVH(this hkcdStaticMeshTreeBaseSection _this)
        {
            if (_this.m_nodes == null || _this.m_nodes.Count == 0)
            {
                return null;
            }

            BVHNode root = new BVHNode();
            root.Min = new Vector3(_this.m_domain.m_min.X, _this.m_domain.m_min.Y, _this.m_domain.m_min.Z);
            root.Max = new Vector3(_this.m_domain.m_max.X, _this.m_domain.m_max.Y, _this.m_domain.m_max.Z);

            var cnode = _this.m_nodes[0];
            if ((cnode.m_data & 0x01) > 0)
            {
                root.Left = buildBVHTree(_this, root.Min, root.Max, 1);
                root.Right = buildBVHTree(_this, root.Min, root.Max, (uint)cnode.m_data & 0xFE);
            }
            else
            {
                root.IsTerminal = true;
                root.Index = (uint)cnode.m_data / 2;
            }

            return root;
        }
    }
}
