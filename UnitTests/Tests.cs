using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HKX2;
using NUnit.Framework;

namespace Tests
{
    public class TestBase
    {
        private static readonly Dictionary<string, short> BotwSectionOffsetForExtension =
            new Dictionary<string, short>
            {
                {"hkcl", 0},
                {"hkrg", 0},
                {"hkrb", 0},
                {"hktmrb", 16},
                {"hknm2", 16}
            };

        protected static string[] GetNxFilePathsByExtension(string extension)
        {
            var rootPath =
                Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory ?? throw new InvalidOperationException(),
                    "../../../../"));
            var botwTestFilesPath = Path.Combine(rootPath, "UnitTests", "Testfiles", "BotW");
            var nxTestFilesPath = Path.Combine(botwTestFilesPath, "Switch");
            return Directory.GetFiles(nxTestFilesPath, $@"*.{extension}", SearchOption.AllDirectories);
        }

        protected static string[] GetNxFilePathsCloth()
        {
            return GetNxFilePathsByExtension("hkcl");
        }

        protected static string[] GetNxFilePathsRagdoll()
        {
            return GetNxFilePathsByExtension("hkrg");
        }

        protected static string[] GetNxFilePathsRigidBody()
        {
            return GetNxFilePathsByExtension("hkrb");
        }

        protected static string[] GetNxFilePathsStaticCompound()
        {
            return GetNxFilePathsByExtension("hksc");
        }

        protected static string[] GetNxFilePathsTeraMeshRigidBody()
        {
            return GetNxFilePathsByExtension("hktmrb");
        }

        protected static string[] GetNxFilePathsNavMesh()
        {
            return GetNxFilePathsByExtension("hknm2");
        }

        public static IHavokObject ReadHKX(byte[] bytes)
        {
            var des = new PackFileDeserializer();
            var br = new BinaryReaderEx(bytes);

            return des.Deserialize(br);
        }

        public static byte[] WriteHKX(IHavokObject root, HKXHeader header)
        {
            var s = new PackFileSerializer();
            var ms = new MemoryStream();
            var bw = new BinaryWriterEx(ms);
            s.Serialize(root, bw, header);
            return ms.ToArray();
        }

        public static List<IHavokObject> ReadBotwHKX(string path)
        {
            return ReadBotwHKX(File.ReadAllBytes(path), path.Split('.').Last());
        }
        
        public static List<IHavokObject> ReadBotwHKX(byte[] bytes, string extension)
        {
            if (extension == "hksc")
            {
                var root1 = (StaticCompoundInfo) ReadHKX(bytes);
                var root2 = (hkRootLevelContainer) ReadHKX(bytes.Skip((int) root1.m_Offset).ToArray());
                return new List<IHavokObject> {root1, root2};
            }

            var root = (hkRootLevelContainer) ReadHKX(bytes);
            return new List<IHavokObject> {root};
        }

        public static byte[] WriteBotwHKX(IReadOnlyList<IHavokObject> hkxFiles, string extension, HKXHeader header)
        {
            if (extension == "hksc")
            {
                var root1 = (StaticCompoundInfo) hkxFiles[0];
                var root2 = (hkRootLevelContainer) hkxFiles[1];

                header.SectionOffset = 0;
                var writtenRoot1 = WriteHKX(root1, header);
                root1.m_Offset = (uint) writtenRoot1.Length;
                writtenRoot1 = WriteHKX(root1, header);

                header.SectionOffset = 16;
                var writtenRoot2 = WriteHKX(root2, header);

                var resultBytes = new byte[writtenRoot1.Length + writtenRoot2.Length];
                Buffer.BlockCopy(writtenRoot1, 0, resultBytes, 0, writtenRoot1.Length);
                Buffer.BlockCopy(writtenRoot2, 0, resultBytes, writtenRoot1.Length, writtenRoot2.Length);
                return resultBytes;
            }

            var root = hkxFiles[0];
            header.SectionOffset = BotwSectionOffsetForExtension[extension];
            var writtenRoot = WriteHKX(root, header);
            return writtenRoot;
        }

        private byte[] RoundtripBotwHKX(byte[] bytes, string extension, HKXHeader header)
        {
            return WriteBotwHKX(ReadBotwHKX(bytes, extension), extension, header);
        }

        private void CompareBytes(byte[] bytesFrom, byte[] bytesTo)
        {
            if (!bytesFrom.SequenceEqual(bytesTo))
            {
                Assert.Warn("Fixups are ordered differently!");
            }
        }

        private void CompareSectionDataAndFixups(byte[] bytesFrom, byte[] bytesTo)
        {
            var des1 = new PackFileDeserializer();
            var des2 = new PackFileDeserializer();

            des1.DeserializePartially(new BinaryReaderEx(bytesFrom));
            des2.DeserializePartially(new BinaryReaderEx(bytesTo));

            Assert.AreEqual(des1._dataSection.SectionData, des2._dataSection.SectionData);
            Assert.AreEqual(des1._dataSection.LocalFixups.OrderBy(fixup => fixup.Src),
                des2._dataSection.LocalFixups.OrderBy(fixup => fixup.Src));
            Assert.AreEqual(des1._dataSection.GlobalFixups.OrderBy(fixup => fixup.Src),
                des2._dataSection.GlobalFixups.OrderBy(fixup => fixup.Src));
            Assert.AreEqual(des1._dataSection.VirtualFixups.OrderBy(fixup => fixup.Src),
                des2._dataSection.VirtualFixups.OrderBy(fixup => fixup.Src));
        }

        public virtual void NxToNx(string path)
        {
            var nxBytes = File.ReadAllBytes(path);
            var nxFromNxBytes = RoundtripBotwHKX(nxBytes, path.Split('.').Last(), HKXHeader.BotwNx());

            CompareBytes(nxBytes, nxFromNxBytes);
            CompareSectionDataAndFixups(nxBytes, nxFromNxBytes);
        }

        public virtual void NxToWiiu(string path)
        {
            var nxBytes = File.ReadAllBytes(path);
            var wiiuFromNxBytes = RoundtripBotwHKX(nxBytes, path.Split('.').Last(), HKXHeader.BotwWiiu());

            var wiiuBytes = File.ReadAllBytes(path.Replace("/Switch/", "/WiiU/"));

            CompareBytes(wiiuBytes, wiiuFromNxBytes);
            CompareSectionDataAndFixups(wiiuBytes, wiiuFromNxBytes);
        }

        public virtual void WiiuToWiiu(string path)
        {
            var wiiuBytes = File.ReadAllBytes(path.Replace("/Switch/", "/WiiU/"));
            var wiiuFromWiiuBytes = RoundtripBotwHKX(wiiuBytes, path.Split('.').Last(), HKXHeader.BotwWiiu());

            CompareBytes(wiiuBytes, wiiuFromWiiuBytes);
            CompareSectionDataAndFixups(wiiuBytes, wiiuFromWiiuBytes);
        }

        public virtual void WiiuToNx(string path)
        {
            var wiiuBytes = File.ReadAllBytes(path.Replace("/Switch/", "/WiiU/"));
            var nxFromWiiuBytes = RoundtripBotwHKX(wiiuBytes, path.Split('.').Last(), HKXHeader.BotwNx());

            var nxBytes = File.ReadAllBytes(path);

            CompareBytes(nxBytes, nxFromWiiuBytes);
            CompareSectionDataAndFixups(nxBytes, nxFromWiiuBytes);
        }
    }

    #region Filetypes
    
    [Parallelizable(ParallelScope.All)]
    public class Cloth : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsCloth))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsCloth))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsCloth))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsCloth))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }

    [Parallelizable(ParallelScope.All)]
    public class Ragdoll : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRagdoll))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRagdoll))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRagdoll))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRagdoll))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }

    [Parallelizable(ParallelScope.All)]
    public class RigidBody : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRigidBody))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRigidBody))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRigidBody))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsRigidBody))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }

    [Parallelizable(ParallelScope.All)]
    public class StaticCompound : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsStaticCompound))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsStaticCompound))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsStaticCompound))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsStaticCompound))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }

    [Parallelizable(ParallelScope.All)]
    public class TeraMeshRigidBody : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsTeraMeshRigidBody))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsTeraMeshRigidBody))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsTeraMeshRigidBody))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsTeraMeshRigidBody))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }

    [Parallelizable(ParallelScope.All)]
    public class NavMesh : TestBase
    {
        [Test]
        [TestCaseSource(nameof(GetNxFilePathsNavMesh))]
        public override void NxToNx(string path)
        {
            base.NxToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsNavMesh))]
        public override void NxToWiiu(string path)
        {
            base.NxToWiiu(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsNavMesh))]
        public override void WiiuToNx(string path)
        {
            base.WiiuToNx(path);
        }

        [Test]
        [TestCaseSource(nameof(GetNxFilePathsNavMesh))]
        public override void WiiuToWiiu(string path)
        {
            base.WiiuToWiiu(path);
        }
    }
    
    #endregion
}