using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using HKX2;
using HKX2.Builders;
using HKX2.Builders.Extensions;

namespace BakeTool
{
    internal static class Ext
    {
        // https://stackoverflow.com/a/22801345
        public static int AddSorted<T>(this List<T> @this, T item) where T : ActorInfo
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return @this.Count - 1;
            }

            if (@this[^1].m_HashId.CompareTo(item.m_HashId) <= 0)
            {
                @this.Add(item);
                return @this.Count - 1;
            }

            if (@this[0].m_HashId.CompareTo(item.m_HashId) >= 0)
            {
                @this.Insert(0, item);
                return 0;
            }

            int index = @this.Select(info => info.m_HashId).ToList().BinarySearch(item.m_HashId);
            if (index < 0)
                index = ~index;
            @this.Insert(index, item);
            return index;
        }
    }

    internal static class Program
    {
        private static List<BVHNode> GetLeaves(BVHNode node)
        {
            var leaves = new List<BVHNode>();

            if (node.IsTerminal)
            {
                leaves.Add(node);
                return leaves;
            }

            leaves.AddRange(GetLeaves(node.Left));
            leaves.AddRange(GetLeaves(node.Right));
            return leaves;
        }

        private static void Main(string[] args)
        {
            var staticCompoundFile = args[0];
            var hashId = Convert.ToUInt32(args[1]);
            var operation = args[2];

            using var rs = File.OpenRead(staticCompoundFile);

            var des = new PackFileDeserializer();
            des.DeserializePartially(new BinaryReaderEx(rs));
            var header = des._header;
            rs.Position = 0;

            var roots = Util.ReadBotwHKX(rs, "hksc");

            var root1 = (StaticCompoundInfo) roots[0];
            var actorInfo = root1.m_ActorInfo;
            var shapeInfo = root1.m_ShapeInfo;

            var root2 = (hkRootLevelContainer) roots[1];
            var staticCompoundShape = (hkpStaticCompoundShape) ((hkpPhysicsData) root2.m_namedVariants[0].m_variant)
                .m_systems[0].m_rigidBodies[0].m_collidable.m_shape;
            var staticCompoundTree = staticCompoundShape.m_tree;

            var decompressed = staticCompoundTree.GetBVH();
            var leaves = GetLeaves(decompressed);

            switch (operation)
            {
                case "add":
                {
                    var hkrbFile = args[3];

                    var translation = new Vector4(Convert.ToSingle(args[4]), Convert.ToSingle(args[5]),
                        Convert.ToSingle(args[6]), 0.5000001f);
                    var rotation = new Quaternion(Convert.ToSingle(args[7]), Convert.ToSingle(args[8]),
                        Convert.ToSingle(args[9]), 1.0f);
                    var scale = new Vector4(Convert.ToSingle(args[10]), Convert.ToSingle(args[11]),
                        Convert.ToSingle(args[12]), 0.5f);

                    using var rs1 = File.OpenRead(hkrbFile);
                    var root = (hkRootLevelContainer) Util.ReadHKX(rs1);
                    var shape = (hkpBvCompressedMeshShape) ((hkpPhysicsData) root.m_namedVariants[0].m_variant)
                        .m_systems[0].m_rigidBodies[0].m_collidable.m_shape;
                    var shapeDomain = shape.m_tree.m_domain;

                    var transformedDomainMin =
                        Vector4.Add(Vector4.Transform(Vector4.Multiply(shapeDomain.m_min, scale), rotation),
                            translation);
                    var transformedDomainMax =
                        Vector4.Add(Vector4.Transform(Vector4.Multiply(shapeDomain.m_max, scale), rotation),
                            translation);

                    // workaround for accuracy issues
                    transformedDomainMin = Vector4.Multiply(transformedDomainMin, 1.5f);
                    transformedDomainMax = Vector4.Multiply(transformedDomainMax, 1.5f);

                    var ai = new ActorInfo
                    {
                        m_HashId = hashId,
                        m_SRTHash = 0,
                        m_ShapeInfoStart = shapeInfo.Count,
                        m_ShapeInfoEnd = shapeInfo.Count
                    };

                    var aiIndex = actorInfo.AddSorted(ai);

                    shapeInfo.Skip(aiIndex).ToList().ForEach(info =>
                    {
                        info.m_ActorInfoIndex++;
                    });
                    
                    shapeInfo.Add(new ShapeInfo
                    {
                        m_ActorInfoIndex = aiIndex,
                        m_BodyLayerType = 0,
                        m_BodyGroup = 0,
                        m_InstanceId = staticCompoundShape.m_instances.Count
                    });

                    staticCompoundShape.m_instances.Add(new hkpStaticCompoundShapeInstance
                    {
                        m_childFilterInfoMask = 0xFFFFFFFF,
                        m_filterInfo = 0,
                        m_shape = shape,
                        m_transform = new Matrix4x4
                        {
                            M11 = translation.X,
                            M12 = translation.Y,
                            M13 = translation.Z,
                            M14 = translation.W,
                            M21 = rotation.X,
                            M22 = rotation.Y,
                            M23 = rotation.Z,
                            M24 = rotation.W,
                            M31 = scale.X,
                            M32 = scale.Y,
                            M33 = scale.Z,
                            M34 = scale.W,
                            M41 = 0f,
                            M42 = 0f,
                            M43 = 0f,
                            M44 = 1f
                        },
                        m_userData = (ulong) (shapeInfo.Count - 1)
                    });

                    var xs = new List<float>
                    {
                        transformedDomainMin.X,
                        transformedDomainMax.X
                    };

                    var ys = new List<float>
                    {
                        transformedDomainMin.Y,
                        transformedDomainMax.Y
                    };

                    var zs = new List<float>
                    {
                        transformedDomainMin.Z,
                        transformedDomainMax.Z
                    };

                    leaves.Add(new BVHNode
                    {
                        Min = new Vector3(xs.Min(), ys.Min(), zs.Min()),
                        Max = new Vector3(xs.Max(), ys.Max(), zs.Max())
                    });

                    break;
                }
                case "remove":
                {
                    var ai = actorInfo.Find(info => info.m_HashId == hashId);
                    var aiIndex = actorInfo.IndexOf(ai);

                    var index = ai.m_ShapeInfoStart;
                    var count = ai.m_ShapeInfoEnd - ai.m_ShapeInfoStart + 1;

                    shapeInfo.RemoveRange(index, count);
                    leaves.RemoveRange(index, count);
                    staticCompoundShape.m_instances.RemoveRange(index, count);
                    actorInfo.Remove(ai);

                    actorInfo.Skip(aiIndex).ToList().ForEach(info =>
                    {
                        info.m_ShapeInfoStart -= count;
                        info.m_ShapeInfoEnd -= count;
                    });

                    shapeInfo.Skip(index).ToList().ForEach(info =>
                    {
                        info.m_ActorInfoIndex--;
                        info.m_InstanceId -= count;
                    });

                    staticCompoundShape.m_instances.Skip(index).ToList().ForEach(instance =>
                    {
                        instance.m_userData -= (ulong) count;
                    });

                    break;
                }
                default:
                {
                    throw new Exception("Invalid operation");
                }
            }

            var domains = new List<Vector3>();
            foreach (var leaf in leaves)
            {
                domains.Add(leaf.Min);
                domains.Add(leaf.Max);
            }

            if (domains.Count > 0)
            {
                var newBVH = BVNode.BuildBVHForDomains(domains.ToArray(), domains.Count / 2);
                var newAxis6 = newBVH[0].BuildAxis6Tree();

                staticCompoundTree.m_nodes = newAxis6;
                staticCompoundTree.m_domain.m_min = new Vector4(newBVH[0].Min, 1f);
                staticCompoundTree.m_domain.m_max = new Vector4(newBVH[0].Max, 1f);

                var decomp = staticCompoundTree.GetBVH();
            }

            using var ws = File.OpenWrite(staticCompoundFile + ".out");
            Util.WriteBotwHKX(roots, header, "hksc", ws);

            return;
        }
    }
}