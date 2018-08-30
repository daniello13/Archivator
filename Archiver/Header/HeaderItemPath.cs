using Archiver.Utils;
using System.Collections.Generic;

namespace Archiver.Header
{
    public class HeaderItemPath
    {
        List<string> pathes;

        public HeaderItemPath()
        {
            pathes = new List<string>();
        }

        public string GetCurrentPath()
        {
            return pathes[pathes.Count - 1];
        }

        public void UpdateCurrentPath(string path)
        {
            pathes.Add(path);
        }

        public void ClearTemporeryPathes(bool with_parent)
        {
            int index = (with_parent == true) ? 0 : 1;
            for (; index < pathes.Count; index++)
                Operations.DeleteFile(pathes[index]);
        }

        public void RemoveLast()
        {
            pathes.RemoveAt(pathes.Count - 1);
        }
    }
}