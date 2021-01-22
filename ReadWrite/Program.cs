using System.IO;
using System.Linq;
using HKX2;
using Tests;

namespace ReadWrite
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var roots = TestBase.ReadBotwHKX(args[0]);
            var outBytes = TestBase.WriteBotwHKX(roots, args[0].Split('.').Last(), HKXHeader.BotwNx());
            
            File.WriteAllBytes(args[0] + ".out", outBytes);
        }
    }
}