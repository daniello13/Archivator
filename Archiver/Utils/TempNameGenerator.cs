using System;
using System.IO;

namespace Archiver.Utils
{
    public static class TempNameGenerator
    {
        public static string GenerateTempNameFromFile(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string result;
            int count = 0;
            do
            {
                result = Path.Combine(dir, String.Format("{0}_{1}.ztemp", count++, name));
            } while (File.Exists(result));
            return result;
        }
    }
}
