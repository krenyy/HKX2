using System.Numerics;

namespace HKX2.Builders.Extensions
{
    public static partial class Extensions
    {
        public static Vector3 DecompressMin(this hkcdStaticTreeCodec3Axis _this, Vector3 parentMin, Vector3 parentMax)
        {
            float x = (_this.m_xyz_0 >> 4) * (float)(_this.m_xyz_0 >> 4) * (1.0f / 226.0f) * (parentMax.X - parentMin.X) + parentMin.X;
            float y = (_this.m_xyz_1 >> 4) * (float)(_this.m_xyz_1 >> 4) * (1.0f / 226.0f) * (parentMax.Y - parentMin.Y) + parentMin.Y;
            float z = (_this.m_xyz_2 >> 4) * (float)(_this.m_xyz_2 >> 4) * (1.0f / 226.0f) * (parentMax.Z - parentMin.Z) + parentMin.Z;
            return new Vector3(x, y, z);
        }

        public static Vector3 DecompressMax(this hkcdStaticTreeCodec3Axis _this, Vector3 parentMin, Vector3 parentMax)
        {
            float x = -((_this.m_xyz_0 & 0x0F) * (float)(_this.m_xyz_0 & 0x0F)) * (1.0f / 226.0f) * (parentMax.X - parentMin.X) + parentMax.X;
            float y = -((_this.m_xyz_1 & 0x0F) * (float)(_this.m_xyz_1 & 0x0F)) * (1.0f / 226.0f) * (parentMax.Y - parentMin.Y) + parentMax.Y;
            float z = -((_this.m_xyz_2 & 0x0F) * (float)(_this.m_xyz_2 & 0x0F)) * (1.0f / 226.0f) * (parentMax.Z - parentMin.Z) + parentMax.Z;
            return new Vector3(x, y, z);
        }
    }
}
