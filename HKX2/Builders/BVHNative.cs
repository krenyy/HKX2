using System.Numerics;
using System.Runtime.InteropServices;

namespace HKX2.Builders
{
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