using System.Text;
using System.IO;
using System.Security.Cryptography;
using Archiver.Utils;

namespace Archiver.Processors
{
    public class EncryptFile : IProcessFile
    {
        byte[] KeyBlock;
        byte[] IV;
        string hashAlgorithm = "SHA1";
        int passwordIterations = 4;
        int keySize = 256;
        byte[] saltValueBytes;

        public void SetKey(string Key)
        {
            saltValueBytes = Encoding.Default.GetBytes("3s1a4l1t5");
            IV = new byte[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 1 };
            int index = 0;
            string key = Key;
            if (key.Length > 8)
                key = key.Substring(0, 8);
            while (key.Length < 8)
                key += IV[index++];
            KeyBlock = Encoding.Default.GetBytes(key);
        }

        public string ProcessExecute(string fileName)
        {
            string outFile = TempNameGenerator.GenerateTempNameFromFile(fileName);
            PasswordDeriveBytes password = new PasswordDeriveBytes(KeyBlock, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, IV);
                using (FileStream fsInput = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsEncrypted = new FileStream(outFile, FileMode.Create, FileAccess.Write))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] bytearrayinput = new byte[fsInput.Length];
                            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                            cryptoStream.Write(bytearrayinput, 0, bytearrayinput.Length);
                            cryptoStream.FlushFinalBlock();
                            cryptoStream.Close();
                        }
                        fsEncrypted.Close();
                    }
                    fsInput.Close();
                }
            }
            return outFile;
        }

        public string BackProcessExecute(string fileName)
        {
            string outFile = TempNameGenerator.GenerateTempNameFromFile(fileName);
            PasswordDeriveBytes password = new PasswordDeriveBytes(KeyBlock, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, IV);
                using (FileStream fsread = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fsread, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryWriter fsDecrypted = new BinaryWriter(new FileStream(outFile, FileMode.Create, FileAccess.Write)))
                        {
                            byte[] file_data = new byte[fsread.Length];
                            int decryptedByteCount = cryptoStream.Read(file_data, 0, file_data.Length);
                            fsDecrypted.Write(file_data, 0, decryptedByteCount);
                            fsDecrypted.Flush();
                            fsDecrypted.Close();
                        }
                        cryptoStream.Close();
                    }
                }
            }
            return outFile;
        }
    }
}