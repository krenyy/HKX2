using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace HKX2.Builders
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct NativeBVHNode
    {
        [FieldOffset(0)] public float minX;
        [FieldOffset(4)] public float maxX;
        [FieldOffset(8)] public float minY;
        [FieldOffset(12)] public float maxY;
        [FieldOffset(16)] public float minZ;
        [FieldOffset(20)] public float maxZ;
        [FieldOffset(24)] private uint isLeafAndPrimitiveCount;

        public bool isLeaf => Convert.ToBoolean(isLeafAndPrimitiveCount & 1);
        public uint primitiveCount => isLeafAndPrimitiveCount >> 1;
        
        [FieldOffset(28)] public uint firstChildOrPrimitive;
    }

    public static class BVHNative
    {
        [DllImport("libNavGen")]
        public static extern bool BuildBVHForMesh([In] Vector3[] verts, int vcount, [In] uint[] indices, int icount);

        [DllImport("libNavGen")]
        public static extern ulong GetNodeSize();

        [DllImport("libNavGen")]
        public static extern ulong GetBVHSize();

        [DllImport("libNavGen")]
        public static extern void GetBVHNodes([In, Out] NativeBVHNode[] buffer);
    }
}