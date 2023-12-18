using AutoMapper;
using System;

namespace OLab.Data.Mappers;

public class Base64Converter : ITypeConverter<string, byte[]>, ITypeConverter<byte[], string>
{
  public byte[] Convert(string source, byte[] destination, ResolutionContext context)
      => System.Convert.FromBase64String(source);

  public string Convert(byte[] source, string destination, ResolutionContext context)
      => System.Convert.ToBase64String(source);
}

public class DateTimeTypeConverter : ITypeConverter<decimal, DateTime>
{
  public DateTime Convert(decimal source, DateTime destination, ResolutionContext context)
  {
    return new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToDouble(source));
  }
}

public class DecimalDateTimeTypeConverter : ITypeConverter<DateTime, decimal>
{
  public decimal Convert(DateTime source, decimal destination, ResolutionContext context)
  {
    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    var diff = source.ToUniversalTime() - origin;
    return System.Convert.ToDecimal(Math.Floor(diff.TotalSeconds));
  }
}
