using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TussentijdsProject
{
    public class Encrytion
    {
        static byte[] key = new byte[16];
        static byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        public static string Encrypt(string keystring, string stringToEncrypt)
        {
            for (int i = 0; i < key.Length; i++)
            {
                byte place = (byte)keystring[i % keystring.Length];
                Console.WriteLine(place);
            }

            byte[] encrypted;

            using (AesManaged aes = new AesManaged())
            {

                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                using (MemoryStream ms = new MemoryStream())
                {

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(stringToEncrypt);
                        encrypted = ms.ToArray();
                    }
                }

                return System.Text.Encoding.Default.GetString(encrypted);

            }

        }

    }
}

