using System.IO;
using zlib;
using Archiver.Utils;

namespace Archiver.Processors
{
    public class CompressFile : IProcessFile
    {
        StreamTransfer transfer = new StreamTransfer();

        public string ProcessExecute(string fileName)
        {
            string outFile = TempNameGenerator.GenerateTempNameFromFile(fileName);
            using (FileStream outFileStream = new FileStream(outFile, FileMode.Create, FileAccess.Write))
            {
                using (ZOutputStream outZStream = new ZOutputStream(outFileStream, zlibConst.Z_BEST_COMPRESSION))
                {
                    using (FileStream inFileStream = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        transfer.Transfer(inFileStream, outZStream);
                    }
                }
            }
            return outFile;
        }

        public string BackProcessExecute(string fileName)
        {
            int data = 0;
            int stopByte = -1;
            string out_file = TempNameGenerator.GenerateTempNameFromFile(fileName);
            using (FileStream outFileStream = new FileStream(out_file, FileMode.Create))
            {
                using (ZInputStream inZStream = new ZInputStream(File.Open(fileName, FileMode.Open, FileAccess.Read)))
                {
                    while (stopByte != (data = inZStream.Read()))
                    {
                        byte _dataByte = (byte)data;
                        outFileStream.WriteByte(_dataByte);
                    }
                    inZStream.Close();
                }
                outFileStream.Close();
            }
            return out_file;
        }
    }
}