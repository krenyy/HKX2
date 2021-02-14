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

            using (var rs = File.OpenRead(args[0]))
            {
                var roots = Util.ReadBotwHKX(rs, extension);
                
                using (var ws = File.OpenWrite($@"{args[0]}.out"))
                {
                    Util.WriteBotwHKX(roots, HKXHeader.BotwNx(), extension, ws);
                }
            }
        }
    }
}