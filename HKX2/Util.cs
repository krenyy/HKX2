using System.Collections.Generic;
using System.IO;

namespace HKX2
{
    public static class Util
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

        public static IHavokObject ReadHKX(Stream stream)
        {
            var des = new PackFileDeserializer();
            var br = new BinaryReaderEx(stream);

            return des.Deserialize(br);
        }

        public static IHavokObject ReadHKX(byte[] bytes) => ReadHKX(new MemoryStream(bytes));

        public static void WriteHKX(IHavokObject root, HKXHeader header, Stream stream)
        {
            var s = new PackFileSerializer();
            var bw = new BinaryWriterEx(stream);

            s.Serialize(root, bw, header);
        }

        public static byte[] WriteHKX(IHavokObject root, HKXHeader header)
        {
            var ms = new MemoryStream();
            WriteHKX(root, header, ms);
            return ms.ToArray();
        }

        public static List<IHavokObject> ReadBotwHKX(Stream stream, string extension)
        {
            if (extension == "hksc")
            {
                var root1 = (StaticCompoundInfo) ReadHKX(stream);

                var ms = new MemoryStream();
                stream.Position = root1.m_Offset;
                stream.CopyTo(ms);
                ms.Position = 0;

                var root2 = (hkRootLevelContainer) ReadHKX(ms);
                return new List<IHavokObject> {root1, root2};
            }

            var root = (hkRootLevelContainer) ReadHKX(stream);
            return new List<IHavokObject> {root};
        }

        public static List<IHavokObject> ReadBotwHKX(byte[] bytes, string extension) =>
            ReadBotwHKX(new MemoryStream(bytes), extension);


        public static void WriteBotwHKX(IReadOnlyList<IHavokObject> hkxFiles, HKXHeader header, string extension,
            Stream stream)
        {
            if (extension == "hksc")
            {
                var root1 = (StaticCompoundInfo) hkxFiles[0];
                var root2 = (hkRootLevelContainer) hkxFiles[1];

                var ms1 = new MemoryStream();
                var ms2 = new MemoryStream();

                // First StaticCompound HKX file doesn't have predicate
                header.SectionOffset = 0;
                // Write to get the length of serialized file - offset of second file
                WriteHKX(root1, header, ms1);
                root1.m_Offset = (uint) ms1.Length;
                ms1.Position = 0;
                WriteHKX(root1, header, ms1);

                header.SectionOffset = 16;
                WriteHKX(root2, header, ms2);

                // Copy the result to stream
                ms1.Position = 0;
                ms2.Position = 0;
                ms1.CopyTo(stream);
                ms2.CopyTo(stream);
                return;
            }

            var root = hkxFiles[0];
            header.SectionOffset = BotwSectionOffsetForExtension[extension];
            WriteHKX(root, header, stream);
        }

        public static byte[] WriteBotwHKX(IReadOnlyList<IHavokObject> hkxFiles, HKXHeader header, string extension)
        {
            var ms = new MemoryStream();
            WriteBotwHKX(hkxFiles, header, extension, ms);
            return ms.ToArray();
        }
    }
}