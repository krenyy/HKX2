using System;
using System.IO;
using System.Linq;
using HKX2;
using Tests;

namespace ReadWrite
{
    internal static class Program
    {
        private static void Main()
        {
            var rootPath =
                Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory ?? throw new InvalidOperationException(), "../../../../"));
            var testFilesPath = Path.Combine(rootPath, "UnitTests", "Testfiles", "BotW");
            var wiiu = Path.Combine(testFilesPath, "WiiU");
            var nx = Path.Combine(testFilesPath, "Switch");

            var path = $@"{wiiu}/Physics/Cloth/UMii_Hylia_Hair_B_002/UMii_Hylia_Hair_B_002_C1.hkcl";
            var extension = path.Split('.').Last();

            var outPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                path.Split('/').Last());

            var hkxFiles = TestBase.ReadBotwHKX(path);
            var outBytes = TestBase.WriteBotwHKX(hkxFiles, extension, HKXHeader.BotwNx());
            
            File.WriteAllBytes(outPath, outBytes);
        }
    }
}