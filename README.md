# HKX2
A standalone customized version of Katalash's HKX2 library for Havok packfile deserialization and serialization used in DSMapStudio.
### Differences
- Supports Breath of the Wild packfiles used on Wii U and Switch.
- Supports conversion of packfiles between Wii U and Switch.
- Padding improvements which allow for perfectly matching files
### Usage
Can be required as a NuGet package: [BotW-HKX2](https://www.nuget.org/packages/BotW-HKX2).
```c#
using HKX2;

namespace PlatformConverter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string inFile = args[0];
            string outFile = args[1];
            string outPlatform = args[3];
            
            HKXHeader header = outPlatform switch
            {
                "wiiu" => HKXHeader.BotwWiiu(),
                "nx" => HKXHeader.BotwNx(),
                _ => throw new Exception()
            };
            
            using (FileStream rs = File.OpenRead(inFile))
            {
                var br = new BinaryReaderEx(rs);
                var des = new PackFileDeserializer();
            
                var root = (hkRootLevelContainer) des.Deserialize(br);
                
                using (FileStream ws = File.Create(outFile))
                {
                    var bw = new BinaryWriterEx(ws);
                    var s = new PackFileSerializer();
                    
                    s.Serialize(root, bw, header);
                }
            }
        }
    }
}
```

### Used libraries
- [bvh](https://github.com/madmann91/bvh)
- [recastnavigation](https://github.com/recastnavigation/recastnavigation)

### Credits
- [katalash](https://github.com/katalash) - The original HKX2 library included in [DSMapStudio](https://github.com/katalash/DSMapStudio)
- [JKAnderson](https://github.com/JKAnderson) - BinaryReaderEx and BinaryWriterEx included in  [SoulsFormats](https://github.com/JKAnderson/SoulsFormats)
