using System;

namespace OLab.Common.Utils;

public class TimeUtils
{
  /// <summary>
  /// Get the current time in UTC without sub-second portion
  /// </summary>
  /// <returns></returns>
  public static DateTime UtcNow()
  {
    var now = DateTime.UtcNow;
    // truncate the sub-secondportion
    now = now.AddTicks(-(now.Ticks % TimeSpan.TicksPerSecond));
    return now;
  }

  /// <summary>
  /// Convert a date time as-is to UTC
  /// </summary>
  /// <param name="source">Source time</param>
  /// <returns></returns>
  public static DateTime? ToUtc(DateTime? source)
  {
    if (source.HasValue)
    {
      if ( source.Value.Kind != DateTimeKind.Utc )
        return DateTime.SpecifyKind(source.Value, DateTimeKind.Utc);
      return source;
    }

    return source;
  }
}
