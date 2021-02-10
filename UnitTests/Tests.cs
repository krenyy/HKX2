using System;
using System.IO;
using System.Linq;
using HKX2;
using NUnit.Framework;

namespace Tests
{
    public class TestBase
    {
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

        private byte[] RoundtripBotwHKX(byte[] bytes, string extension, HKXHeader header)
        {
            return Util.WriteBotwHKX(Util.ReadBotwHKX(bytes, extension), header, extension);
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