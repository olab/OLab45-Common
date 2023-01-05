using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Text;

namespace OLabWebAPI.Utils
{
  public static class Conversions
  {
    public static uint? OptionalIdSafeAssign(uint source)
    {
      if (source > 0)
        return source;
      return null;
    }

    public static uint? OptionalIdSafeAssign(uint? source)
    {
      if ((source > 0) && (source.HasValue))
        return source;
      return null;
    }

    /// <summary>
    /// Decode Base64 string
    /// </summary>
    /// <param name="data">Dynamic source (usually an XML element)</param>
    /// <param name="returnClearTextIfError">Return string as cleartext if there's an error</param>
    /// <returns>Decoded string</returns>
    public static string Base64Decode(dynamic data, bool returnClearTextIfError = false)
    {
      string name = "";
      try
      {
        name = data.Name.ToString();
      }
      catch (RuntimeBinderException)
      {
      }

      try
      {
        byte[] binary = Convert.FromBase64String(data.Value);
        return Encoding.Default.GetString(binary);
      }
      catch (FormatException)
      {
        if (!returnClearTextIfError)
        {
          if (string.IsNullOrEmpty(name))
            throw new Exception($"cannot decode '{data.Value.ToString()}'");
          else
            throw new Exception($"cannot decode '{name}' = '{data.Value.ToString()}'");
        }
      }

      return data.Value.ToString();
    }

    /// <summary>
    /// Decode Base64 string
    /// </summary>
    /// <param name="data">PHP-compatible base64 source string</param>
    /// <returns>Decoded string</returns>
    public static string Base64Decode(string data)
    {
      byte[] binary = Convert.FromBase64String(data);
      return Encoding.Default.GetString(binary);
    }

  }
}