using System.IO;
using System.Linq;
using HKX2;

namespace ReadWrite
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var extension = args[0].Split('.').Last();
            
            var roots = Util.ReadBotwHKX(File.OpenRead(args[0]), extension);
            Util.WriteBotwHKX(roots, HKXHeader.BotwNx(), extension, File.OpenWrite(args[0] + ".out"));
        }
    }
}