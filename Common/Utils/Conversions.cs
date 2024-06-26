using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Buffers.Text;
using System.Text;

namespace OLab.Api.Utils;

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
    var name = "";
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
  /// <param name="source">PHP-compatible base64 source string</param>
  /// <returns>Decoded string</returns>
  public static string Base64Decode(string source, bool isBase64Encoded = true )
  {
    var data = source;

    try
    {
      if (isBase64Encoded && !string.IsNullOrEmpty(source))
      {
        var base64Bytes = Convert.FromBase64String(source);
        data = Encoding.Default.GetString(base64Bytes);
      }
    }
    catch (Exception)
    {
    }

    return data;
  }

  /// <summary>
  /// Get current time in epoch seconds
  /// </summary>
  /// <returns>Seconds (including fractional) since epoch</returns>
  public static decimal GetCurrentUnixTime()
  {
    var span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    var unixTime = span.TotalSeconds;
    return (decimal)unixTime;
  }

  /// <summary>
  /// Convert epoch seconds to a DateTime
  /// </summary>
  /// <param name="epochSeconds"></param>
  /// <returns>DateTime</returns>
  public static DateTime GetTime(decimal epochSeconds)
  {
    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)epochSeconds);
    return dateTimeOffset.DateTime;
  }

}