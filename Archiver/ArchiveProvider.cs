using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Archiver.Header;
using Archiver.Utils;
using Archiver.Processors;

namespace Archiver
{
    public enum OperationErrorAction
    {
        Abort,
        Ignore,
        IgnoreAll,
        Replay
    }

    public class CompressorOption
    {
        public string Output { get; set; }
        public string Password { get; set; }
        public List<string> IncludePath = new List<string>();
    }

    public delegate void ArchiveProviderProcessHandler(string message);

    public class ArchiveProvider
    {
        public event ArchiveProviderProcessHandler ProcessMessages;
        StreamTransfer transfer = new StreamTransfer();
        Header.Header header;
        [Flags]
        enum PrimeArchiverType : byte
        {
            Nothing = 0x0,
            Password = 0x1
        }
        List<IProcessFile> processors = new List<IProcessFile>();

        public void Compress(CompressorOption option)
        {
            bool ignore_all = false;
            PrimeArchiverType type = PrimeArchiverType.Nothing;
            processors.Clear();
            processors.Add(new CompressFile());
            if (!String.IsNullOrEmpty(option.Password))
            {
                type |= PrimeArchiverType.Password;
                EncryptFile ef = new EncryptFile();
                ef.SetKey(option.Password);
                processors.Add(ef);
            }
            header = new Header.Header();
            HeaderItemPath hip = new HeaderItemPath();
            try
            {
                string temp_archive = TempNameGenerator.GenerateTempNameFromFile(option.Output);
                using (FileStream archiveStream = new FileStream(temp_archive, FileMode.Create, FileAccess.Write))
                {
                    IncludesPathCreate(option);
                    for (int i = 0; i < header.Items.Count; i++)
                    {
                        HeaderItemPath hip_file = new HeaderItemPath();
                        try
                        {
                            Process(header.Items[i].AbsolutePath);
                            if (header.Items[i].Length != 0)
                            {

                                hip_file.UpdateCurrentPath(header.Items[i].AbsolutePath);
                                foreach (IProcessFile processor in processors)
                                {
                                    hip_file.UpdateCurrentPath(processor.ProcessExecute(hip_file.GetCurrentPath()));
                                }
                                using (FileStream fr = new FileStream(hip_file.GetCurrentPath(), FileMode.Open, FileAccess.Read))
                                {
                                    header.Items[i].SetLentgh(fr.Length);
                                    transfer.Transfer(fr, archiveStream);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ignore_all) continue;
                            OperationErrorAction action = OperationErrorAction.Abort;
                            switch (action)
                            {
                                case OperationErrorAction.Abort:
                                    throw ex;
                                case OperationErrorAction.Ignore:
                                    continue;
                                case OperationErrorAction.IgnoreAll:
                                    ignore_all = true;
                                    continue;
                                case OperationErrorAction.Replay:
                                    i--;
                                    break;
                            }
                        }
                        finally
                        {
                            hip_file.ClearTemporeryPathes(false);
                        }
                    }
                }
                string header_path = TempNameGenerator.GenerateTempNameFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "header.dat"));
                hip.UpdateCurrentPath(header_path);
                using (FileStream fs = new FileStream(hip.GetCurrentPath(), FileMode.Create, FileAccess.Write))
                {
                    new StreamTransfer().Transfer(new MemoryStream(header.ToArray()), fs);
                }
                foreach (IProcessFile processor in processors)
                {
                    hip.UpdateCurrentPath(processor.ProcessExecute(hip.GetCurrentPath()));
                }
                using (FileStream endArchiveStream = new FileStream(option.Output, FileMode.Create, FileAccess.Write))
                {
                    using (FileStream fr = new FileStream(hip.GetCurrentPath(), FileMode.Open, FileAccess.Read))
                    {
                        endArchiveStream.WriteByte((byte)type);
                        int after_processors_header_length = (int)fr.Length;
                        endArchiveStream.Write(BitConverter.GetBytes(after_processors_header_length), 0, sizeof(int));
                        transfer.Transfer(fr, endArchiveStream);
                    }
                    using (FileStream fr = new FileStream(temp_archive, FileMode.Open, FileAccess.Read))
                    {
                        transfer.Transfer(fr, endArchiveStream);
                    }
                    Operations.DeleteFile(temp_archive);
                }
            }
            catch (Exception ex)
            {
                Process(ex.Message);
            }
            finally
            {
                hip.ClearTemporeryPathes(true);
                processors.Clear();
            }
        }

        void IncludesPathCreate(CompressorOption option)
        {
            List<string> includes = new List<string>();
            bool alreadyUse = false;
            foreach (string str in option.IncludePath)
            {
                alreadyUse = false;
                for (int i = 0; i < includes.Count; i++)
                {
                    if (str.IndexOf(includes[i], StringComparison.OrdinalIgnoreCase) >= 0 || includes[i].IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        alreadyUse = true;
                        if (str.Length < includes[i].Length)
                            includes[i] = str;
                    }
                }
                if (!alreadyUse)
                    includes.Add(str);
            }
            foreach (string path in includes)
            {
                AppendToArchive(path, option);
            }
            return;
        }

        void AppendToArchive(string path, CompressorOption option)
        {
            if (Directory.Exists(path))
                AppendFolderToArchive(Path.GetDirectoryName(path), path, option);
            else if (File.Exists(path))
                AppendFileToArchive(Path.GetDirectoryName(path), path, option);
        }

        void AppendFolderToArchive(string root, string path, CompressorOption option)
        {
            string relationName = Operations.GetPathDifferent(root, path);
            header.Insert(new HeaderItem(path, relationName, 0));
            string[] files = Operations.ListFiles(path);
            if (files != null)
                foreach (string file in files)
                    AppendFileToArchive(root, file, option);
            string[] folders = Operations.ListDirectoies(path);
            if (folders != null)
                foreach (string folder in folders)
                    AppendFolderToArchive(root, folder, option);
        }

        void AppendFileToArchive(string root, string path, CompressorOption option)
        {
            string relationPath = Operations.GetPathDifferent(root, path);
            HeaderItem item = new HeaderItem(path, relationPath, 1);
            header.Insert(item);
        }

        public void Decompress(string source, string output, string password)
        {
            CompressorOption option = new CompressorOption();
            processors.Clear();
            FileStream fs = null;
            BinaryReader br = null;
            Header.Header unpack_header = new Header.Header();
            try
            {
                using (fs = new FileStream(source, FileMode.Open, FileAccess.Read))
                {
                    using (br = new BinaryReader(fs))
                    {
                        PrimeArchiverType type = (PrimeArchiverType)br.ReadByte();
                        if ((type & PrimeArchiverType.Password) == PrimeArchiverType.Password)
                        {
                            EncryptFile ef = new EncryptFile();
                            ef.SetKey(password);
                            processors.Add(ef);
                        }
                        processors.Add(new CompressFile());
                        byte[] header_length_arr = br.ReadBytes(sizeof(int));
                        byte[] header_arr = br.ReadBytes(BitConverter.ToInt32(header_length_arr, 0));
                        string header_path = TempNameGenerator.GenerateTempNameFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "header.dat"));
                        HeaderItemPath hip_header = new HeaderItemPath();
                        hip_header.UpdateCurrentPath(header_path);
                        using (FileStream fw = new FileStream(header_path, FileMode.Create, FileAccess.Write))
                        {
                            transfer.Transfer(new MemoryStream(header_arr), fw);
                        }
                        foreach (IProcessFile processor in processors)
                        {
                            hip_header.UpdateCurrentPath(processor.BackProcessExecute(hip_header.GetCurrentPath()));
                        }
                        using (FileStream fr = new FileStream(hip_header.GetCurrentPath(), FileMode.Open, FileAccess.Read))
                        {
                            byte[] header_body = new byte[fr.Length];
                            int count = fr.Read(header_body, 0, (int)fr.Length);
                            unpack_header.Parse(header_body);
                        }
                        hip_header.ClearTemporeryPathes(true);
                        foreach (HeaderItem item in unpack_header.Items)
                        {
                            if (item.Length == 0)
                            {
                                string full_path = Path.Combine(output, item.RelativePath);
                                Process(full_path);
                                Operations.CreateDirectory(full_path);
                            }
                            else
                            {
                                string full_path = Path.Combine(output, item.RelativePath);
                                Process(full_path);
                                byte[] file_body = br.ReadBytes((int)item.Length);
                                HeaderItemPath hip_file = new HeaderItemPath();
                                hip_file.UpdateCurrentPath(TempNameGenerator.GenerateTempNameFromFile(full_path));
                                using (FileStream fw = new FileStream(hip_file.GetCurrentPath(), FileMode.Create, FileAccess.Write))
                                {
                                    transfer.Transfer(new MemoryStream(file_body), fw);
                                }
                                foreach (IProcessFile processor in processors)
                                {
                                    hip_file.UpdateCurrentPath(processor.BackProcessExecute(hip_file.GetCurrentPath()));
                                }
                                Operations.MoveFile(hip_file.GetCurrentPath(), full_path);
                                hip_file.RemoveLast();
                                hip_file.ClearTemporeryPathes(true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Process(ex.Message);
            }
            finally
            {
                if (br != null)
                    br.Close();
                if (fs != null)
                    fs.Close();
            }
        }

        private void Process(string msg)
        {
            ProcessMessages?.Invoke(String.Format("{0}: {1}", DateTime.Now.ToString(), msg));
        }
    }
}