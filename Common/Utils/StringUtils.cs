using Newtonsoft.Json;
using OLab.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OLab.Api.Utils;

public class StringUtils
{

  public static string TruncateToJsonObject<T>(T phys, int maxDepth)
  {
    var json = JsonConvert.SerializeObject(
      new List<T> { phys },
      new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );

    return SerializerUtilities.TruncateJsonToDepth( json, maxDepth + 1 );
  }

  public static string TruncateToJsonObject<T>(IList<T> physList, int maxDepth)
  {
    var json = JsonConvert.SerializeObject(
      physList,
      new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );

    return SerializerUtilities.TruncateJsonToDepth( json, maxDepth + 1 );
  }

  public static string GenerateCheckSum(string plainText)
  {
    var sum = 0;
    var bytes = Encoding.ASCII.GetBytes( plainText );
    foreach ( var singleByte in bytes )
      sum += singleByte * 1013;

    return sum.ToString( "X" );
  }

  /// <summary>
  /// Strip out unicode characters from a string
  /// </summary>
  /// <param name="str">Source string</param>
  /// <returns>Cleanzed string</returns>
  public static string StripUnicode(string str)
  {
    return Regex.Replace( str, @"[^\u0000-\u007F]+", string.Empty );
  }

  public static string EncryptString(string key, string plainText)
  {
    var iv = new byte[ 16 ];
    byte[] array;

    using ( var aes = Aes.Create() )
    {
      aes.Key = Encoding.UTF8.GetBytes( key );
      aes.IV = iv;

      var encryptor = aes.CreateEncryptor( aes.Key, aes.IV );

      using var memoryStream = new MemoryStream();
      using var cryptoStream = new CryptoStream( memoryStream, encryptor, CryptoStreamMode.Write );
      using ( var streamWriter = new StreamWriter( cryptoStream ) )
      {
        streamWriter.Write( plainText );
      }

      array = memoryStream.ToArray();
    }

    return Convert.ToBase64String( array );
  }

  public static string DecryptString(string key, string cipherText)
  {
    var iv = new byte[ 16 ];
    var buffer = Convert.FromBase64String( cipherText );

    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes( key );
    aes.IV = iv;
    var decryptor = aes.CreateDecryptor( aes.Key, aes.IV );

    using var memoryStream = new MemoryStream( buffer );
    using var cryptoStream = new CryptoStream( memoryStream, decryptor, CryptoStreamMode.Read );
    using var streamReader = new StreamReader( cryptoStream );
    return streamReader.ReadToEnd();
  }

  /// <summary>
  /// Generates a random string of the specified length using lowercase letters and digits.
  /// </summary>
  /// <param name="length">The length of the random string to generate.</param>
  /// <returns>A random string of the specified length.</returns>
  public static string GenerateRandomString(int length)
  {
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();
    return new string( Enumerable.Repeat( chars, length )
      .Select( s => s[ random.Next( s.Length ) ] ).ToArray() );
  }
}
