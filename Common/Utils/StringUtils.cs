using Dawn;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OLab.Api.Utils;

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

  /// <summary>
  /// Strip out unicode characters from a string
  /// </summary>
  /// <param name="str">Source string</param>
  /// <returns>Cleanzed string</returns>
  public static string StripUnicode(string str)
  {
    return Regex.Replace(str, @"[^\u0000-\u007F]+", string.Empty);
  }

  public static string EncryptString(string key, string plainText)
  {
    Guard.Argument(key).NotNull(nameof(key));
    Guard.Argument(plainText).NotNull(nameof(plainText));

    // ensure key is 32 bytes
    while (key.Length < 32)
      key += key;
    key = key[..32];

    var iv = new byte[16];
    byte[] array;

    using (var aes = Aes.Create())
    {
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = iv;

      var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

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

    Guard.Argument(key).NotNull(nameof(key));
    Guard.Argument(cipherText).NotNull(nameof(cipherText));

    // ensure key is 32 bytes
    while (key.Length < 32)
      key += key;
    key = key[..32];

    var iv = new byte[16];
    var buffer = Convert.FromBase64String(cipherText);

    using (var aes = Aes.Create())
    {
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = iv;
      var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

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
