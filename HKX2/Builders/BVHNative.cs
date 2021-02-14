using System.Numerics;
using System.Runtime.InteropServices;

namespace HKX2.Builders
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeBVHNode
    {
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;
        public uint primitiveCount;
        public uint firstChildOrPrimitive;
        
        public bool isLeaf => primitiveCount > 0;
    }

    public static class BVHNative
    {
        [DllImport("libNavGen")]
        public static extern bool BuildBVHForDomains([In] Vector3[] domains, int domainCount);
        
        [DllImport("libNavGen")]
        public static extern bool BuildBVHForMesh([In] Vector3[] vertices, [In] uint[] indices, int icount);

        [DllImport("libNavGen")]
        public static extern ulong GetNodeSize();

        [DllImport("libNavGen")]
        public static extern ulong GetBVHSize();

        [DllImport("libNavGen")]
        public static extern void GetBVHNodes([In, Out] NativeBVHNode[] buffer);
    }
}