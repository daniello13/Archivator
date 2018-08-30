using System;
using System.Text;
using System.IO;

namespace Archiver.Header
{
    public class HeaderItem
    {
        string relativePath = string.Empty;
        string absolutePath = string.Empty;
        long length = 0;
        int item_length = 0;

        public string RelativePath
        {
            get { return relativePath; }
        }

        public string AbsolutePath
        {
            get { return absolutePath; }
        }

        public long Length
        {
            get { return length; }
        }

        public int ItemLength
        {
            get { return item_length; }
        }

        public HeaderItem()
        {

        }

        public HeaderItem(string _AbsolutePath, string _RelativePath, long _Length)
        {
            relativePath = _RelativePath ?? String.Empty;
            absolutePath = _AbsolutePath ?? String.Empty;
            length = _Length;
            CalculateItemLength();
        }

        public byte[] ToArray()
        {
            int index = 0;
            byte[] result = new byte[item_length];
            int rel_length = relativePath.Length;
            int abs_length = absolutePath.Length;
            Array.Copy(BitConverter.GetBytes(item_length), 0, result, index, sizeof(int));
            index += sizeof(int);
            Array.Copy(BitConverter.GetBytes(abs_length), 0, result, index, sizeof(int));
            index += sizeof(int);
            Array.Copy(Encoding.Default.GetBytes(absolutePath), 0, result, index, abs_length);
            index += abs_length;
            Array.Copy(BitConverter.GetBytes(rel_length), 0, result, index, sizeof(int));
            index += sizeof(int);
            Array.Copy(Encoding.Default.GetBytes(relativePath), 0, result, index, rel_length);
            index += rel_length;
            Array.Copy(BitConverter.GetBytes(length), 0, result, index, sizeof(long));
            return result;
        }

        public void Parse(byte[] array)
        {
            byte[] int_arr_buffer = new byte[sizeof(int)];
            byte[] long_arr_buffer = new byte[sizeof(long)];
            int int_buffer;
            using (MemoryStream ms = new MemoryStream(array))
            {
                ms.Read(int_arr_buffer, 0, sizeof(int));
                int_buffer = BitConverter.ToInt32(int_arr_buffer, 0);
                if (array.Length != int_buffer)
                    throw new Exception("Заголовок поврежден. Реальная длина заголовка не совпадает с записанной.");
                ms.Read(int_arr_buffer, 0, sizeof(int));
                int_buffer = BitConverter.ToInt32(int_arr_buffer, 0);
                if (int_buffer < 0)
                    throw new Exception("Заголовок поврежден. Указана отрицательная длина абсолютного пути.");
                byte[] abs_array = new byte[int_buffer];
                ms.Read(abs_array, 0, int_buffer);
                absolutePath = Encoding.Default.GetString(abs_array);
                ms.Read(int_arr_buffer, 0, sizeof(int));
                int_buffer = BitConverter.ToInt32(int_arr_buffer, 0);
                if (int_buffer < 0)
                    throw new Exception("Заголовок поврежден. Указана отрицательная длина относительного пути.");
                byte[] rel_array = new byte[int_buffer];
                ms.Read(rel_array, 0, int_buffer);
                relativePath = Encoding.Default.GetString(rel_array);
                ms.Read(long_arr_buffer, 0, sizeof(long));
                length = BitConverter.ToInt64(long_arr_buffer, 0);
                if (length < 0)
                    throw new Exception("Заголовок поврежден. Указана отрицательный размер содержимого.");
            }
            CalculateItemLength();
        }

        void CalculateItemLength()
        {
            item_length = relativePath.Length + absolutePath.Length + sizeof(long) + sizeof(int) * 3;
        }

        public void SetLentgh(long _length)
        {
            length = _length;
        }
    }
}