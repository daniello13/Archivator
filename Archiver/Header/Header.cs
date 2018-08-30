using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Header
{
    public class Header
    {
        List<HeaderItem> items;
        public int Length
        {
            get
            {
                int length = sizeof(int);
                foreach (HeaderItem item in items)
                    length += item.ItemLength;
                return length;
            }
        }

        public List<HeaderItem> Items
        {
            get { return items; }
        }

        public Header()
        {
            items = new List<HeaderItem>();
        }

        public void Insert(HeaderItem item)
        {
            items.Add(item);
        }

        public byte[] ToArray()
        {
            int length = sizeof(int);
            foreach (HeaderItem item in items)
                length += item.ItemLength;
            byte[] result = new byte[length];
            int index = 0;
            Array.Copy(BitConverter.GetBytes(length), 0, result, index, sizeof(int));
            index += sizeof(int);
            foreach (HeaderItem item in items)
            {
                Array.Copy(item.ToArray(), 0, result, index, item.ItemLength);
                index += item.ItemLength;
            }
            return result;
        }

        public void Parse(byte[] array)
        {
            if (array.Length <= 0)
                throw new Exception("Невозможно распознать заголовок архива, в переданном массиве отсутствуют данные.");
            items.Clear();
            byte[] int_arr_buf = new byte[sizeof(int)];
            int int_buf;
            int length;
            using (MemoryStream ms = new MemoryStream(array))
            {
                ms.Read(int_arr_buf, 0, sizeof(int));
                length = BitConverter.ToInt32(int_arr_buf, 0);
                if (length > ms.Length)
                {
                    throw new Exception("Некорректный заголовок. Записанная длина заголовка превышает размер файла.");
                }
                while (ms.Position < ms.Length)
                {
                    ms.Read(int_arr_buf, 0, sizeof(int));
                    int_buf = BitConverter.ToInt32(int_arr_buf, 0);
                    byte[] item_array = new byte[int_buf];
                    ms.Read(item_array, sizeof(int), int_buf - sizeof(int));
                    Array.Copy(int_arr_buf, 0, item_array, 0, sizeof(int));
                    HeaderItem item = new HeaderItem();
                    item.Parse(item_array);
                    items.Add(item);
                }
            }
        }
    }
}
