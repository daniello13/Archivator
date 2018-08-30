using System;
using System.IO;

namespace Archiver.Utils
{
    public delegate byte[] StreamTransferBlockHandler(byte[] body, string arg);

    public class StreamTransfer
    {
        public StreamTransferBlockHandler ProcessBlock;
        public int BlockSize = 16384;
        public string ProcessBlockArgs;

        public int Transfer(Stream source, Stream destination)
        {
            int count = 0;
            if (!source.CanRead)
                throw new Exception("Исходящий поток недоступен для чтения");
            int length;
            byte[] buffer = new byte[BlockSize];
            length = BlockSize;
            while (length == BlockSize && (length = source.Read(buffer, 0, BlockSize)) != 0)
            {
                if (length < BlockSize)
                    Array.Resize<byte>(ref buffer, length);
                if (ProcessBlock != null)
                    buffer = ProcessBlock(buffer, ProcessBlockArgs);
                destination.Write(buffer, 0, buffer.Length);
                count += buffer.Length;
            }
            destination.Flush();
            return count;
        }
    }
}