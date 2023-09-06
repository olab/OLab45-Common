using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OLab.Api.Utils
{
  public class StringUtils
  {
    public static string GenerateCheckSum(string plainText)
    {
      var sum = 0;
      var bytes = Encoding.ASCII.GetBytes(plainText);
      foreach (var singleByte in bytes)
        sum += singleByte * 1013;

      return sum.ToString("X");
    }

    public static string EncryptString(string key, string plainText)
    {
      var iv = new byte[16];
      byte[] array;

      using (var aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (var memoryStream = new MemoryStream())
        {
          using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
              streamWriter.Write(plainText);
            }

            array = memoryStream.ToArray();
          }
        }
      }

      return Convert.ToBase64String(array);
    }

    public static string DecryptString(string key, string cipherText)
    {
      var iv = new byte[16];
      var buffer = Convert.FromBase64String(cipherText);

      using (var aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (var memoryStream = new MemoryStream(buffer))
        {
          using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
          {
            using (var streamReader = new StreamReader(cryptoStream))
            {
              return streamReader.ReadToEnd();
            }
          }
        }
      }
    }
  }
}
