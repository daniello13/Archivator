using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archiver.Utils
{
    public static class Operations
    {
        public static void Collect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static string[] ListFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path);
            }
            catch
            {
                return null;
            }
        }

        public static string[] ListFiles(string path, string mask)
        {
            try
            {
                return Directory.GetFiles(path, mask, SearchOption.TopDirectoryOnly);
            }
            catch
            {
                return null;
            }
        }

        public static string[] RecursiveListFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            catch
            {
                return null;
            }
        }

        public static string[] RecursiveListFiles(string path, string mask)
        {
            try
            {
                return Directory.GetFiles(path, mask, SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось удалить файл {0}, причина: {1}", path, ex.Message));
            }
        }

        public static void MoveFile(string src, string dest)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(dest)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Move(src, dest);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось переместить файл из {0} в {1}, причина: {2}", src, dest, ex.Message));
            }
        }

        public static void CopyFile(string src, string dest)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(dest)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(src, dest, true);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось копировать файл из {0} в {1}, причина: {2}", src, dest, ex.Message));
            }
        }

        public static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось удалить каталог {0}, причина: {1}", path, ex.Message));
            }
        }

        public static void StrongDeleteDirectory(string path)
        {
            try
            {
                string[] files = Directory.GetFiles(path);
                if (files != null)
                    for (int i = 0; i < files.Length; i++)
                        DeleteFile(files[i]);
                string[] catalog = Directory.GetDirectories(path);
                if (catalog != null)
                    for (int i = 0; i < catalog.Length; i++)
                        StrongDeleteDirectory(catalog[i]);
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось рекурсивно удалить каталог {0}, причина: {1}", path, ex.Message));
            }
        }

        public static string[] ListDirectoies(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch
            {
                return null;
            }
        }

        public static string[] ListDirectoies(string path, string mask)
        {
            try
            {
                return Directory.GetDirectories(path, mask);
            }
            catch
            {
                return null;
            }
        }

        public static string[] RecursiveListDirectoies(string path)
        {
            try
            {
                return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            }
            catch
            {
                return null;
            }
        }

        public static string[] RecursiveListDirectoies(string path, string mask)
        {
            try
            {
                return Directory.GetDirectories(path, mask, SearchOption.AllDirectories);
            }
            catch
            {
                return null;
            }
        }

        public static void MoveDirectory(string src, string dest)
        {
            try
            {
                Directory.Move(src, dest);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось переместить каталог {0} в {1}, причина: {2}", src, dest, ex.Message));
            }
        }

        public static void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось создать каталог {0}, причина: {1}", path, ex.Message));
            }
        }

        public static string[] GetDirectoriesFromPath(string path)
        {
            List<string> result = new List<string>();
            string buffer = String.Empty;
            Match root = Regex.Match(path, @"^\w:\\", RegexOptions.Singleline);
            MatchCollection parts = Regex.Matches(path, @"\\[\w\.\s]{1,}", RegexOptions.Singleline);
            if (root.Success)
            {
                buffer = root.Value;
            }
            else
            {
                buffer = "\\\\";
            }
            foreach (Match match in parts)
            {
                buffer = Path.Combine(buffer, match.Value.Trim('\\'));
                result.Add(buffer);
            }
            return result.ToArray();
        }

        public static void CreateFullPathDirectory(string path)
        {
            string[] directories = GetDirectoriesFromPath(path);
            foreach (string dir in directories)
                CreateDirectory(dir);
        }

        public static string GetPathDifferent(string RootPath, string Path)
        {
            if (Path.IndexOf(RootPath) == 0)
            {
                return Path.Remove(0, RootPath.Length).Trim('\\');
            }
            return Path;
        }
    }
}