using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OLab.Api.Dto;

public class DynamicScopedObject
{
  [JsonProperty( "counters" )]
  public List<CountersDto> Counters { get; set; }

  public DynamicScopedObject()
  {
    Counters = new List<CountersDto>();
  }
}

public class ServerDynamicScopedObjects : DynamicScopedObject
{
  public ServerDynamicScopedObjects()
  {
  }
}

public class MapDynamicScopedObjects : DynamicScopedObject
{
  public MapDynamicScopedObjects()
  {
  }
}

public class NodeDynamicScopedObjects : DynamicScopedObject
{
  public NodeDynamicScopedObjects()
  {
  }
}

public class CounterDynamicScopedObjects : DynamicScopedObject
{
  public CounterDynamicScopedObjects()
  {
  }
}

public class DynamicScopedObjectsDto
{
  [JsonProperty( "newPlay" )]
  public bool NewPlay { get; set; }

  [JsonProperty( "updatedAt" )]
  public DateTime UpdatedAt { get; private set; }

  [JsonProperty( "checksum" )]
  public string Checksum { get; set; }

  //[JsonProperty( "counters" )]
  //public CounterDynamicScopedObjects Counters { get; set; }
  [JsonProperty( "counters" )]
  public List<CountersDto> Counters { get; set; }

  [JsonProperty( "nodesVisitedList" )]
  public IList<uint> NodesVisited { get; set; }

  public DynamicScopedObjectsDto()
  {
    UpdatedAt = DateTime.Now;
    //Counters = new CounterDynamicScopedObjects();
    Counters = new List<CountersDto>();
    NodesVisited = new List<uint>();
  }

  public bool IsEmpty()
  {
    return Counters.Count == 0;
  }

  /// <summary>
  /// Verify a checksum is valid for a given object
  /// </summary>
  /// <returns>true/false</returns>
  public bool IsValid()
  {
    if ( IsEmpty() )
      return true;

    var expectedCheckSum = GenerateChecksum();
    return expectedCheckSum == Checksum;
  }

  /// <summary>
  /// Generate a verification checksum on the dynamic scope
  /// </summary>
  /// <returns>checksum string</returns>
  public string GenerateChecksum()
  {
    var plainText = GetObjectPlainText();
    var cypherText = GetStringHash( plainText );
    return cypherText;
  }

  public int GetPlainTextBytes(string plainText)
  {
    var bytes = Encoding.ASCII.GetBytes( plainText );
    var byteSum = 0;

    foreach ( var singleByte in bytes )
      byteSum += singleByte;

    return byteSum;
  }

  /// <summary>
  /// Calculate a hash for a given string
  /// </summary>
  /// <param name="key">String to hash</param>
  /// <returns>MD5 hash</returns>
  private string GetStringHash(string plaintext)
  {
    var cypherString = StringUtils.GenerateCheckSum( plaintext );
    return cypherString;
  }

  /// <summary>
  /// Refresh the object checksum
  /// </summary>
  public void RefreshChecksum()
  {
    Checksum = GenerateChecksum();
  }

  public string GetObjectPlainText()
  {
    var counterValues = UpdatedAt.ToString() + "/";

    foreach ( var counter in Counters.OrderBy( x => x.Id ) )
      counterValues += counter.Value + "/";

    return counterValues;

  }

  /// <summary>
  /// ReadAsync counter from scoped object lists
  /// </summary>
  /// <param name="id"></param>
  /// <param name="scopeType">(optional) desired scope level</param>
  /// <returns></returns>
  public CountersDto GetCounter(uint id, string scopeType = "")
  {
    CountersDto dto = null;

    if ( string.IsNullOrEmpty( scopeType ) )
      dto = Counters.FirstOrDefault( x => x.Id == id );

    return dto;
  }

  public string ToJson()
  {
    var rawJson = System.Text.Json.JsonSerializer.Serialize( this );
    return JToken.Parse( rawJson ).ToString( Formatting.Indented );
  }

  public void Dump(IOLabLogger logger, string prefix)
  {
    var message = $"{prefix}{Environment.NewLine}";

    if ( IsEmpty() )
    {
      message += $"IsEmpty{Environment.NewLine}";
      return;
    }

    var plainText = GetObjectPlainText();
    message += $"Text:   {plainText}{Environment.NewLine}";
    message += $"Bytes:  {GetPlainTextBytes( plainText )}{Environment.NewLine}";

    foreach ( var counter in Counters )
      message += $"Counter:    {counter}{Environment.NewLine}";

    message += $"Update  {UpdatedAt.ToString()}{Environment.NewLine}";
    message += $"ChkSum  {Checksum}{Environment.NewLine}";

    logger.LogInformation( message );

  }

  /// <summary>
  /// Update counter value
  /// </summary>
  /// <param name="counterDto"></param>
  /// <exception cref="NotImplementedException"></exception>
  public void UpdateCounter(IOLabLogger logger, CountersDto counterDto)
  {
    CountersDto responseCounterDto = null;

    if ( counterDto == null )
    {
      logger.LogDebug( $"no counter to update" );
      return;
    }

    responseCounterDto = Counters.FirstOrDefault( x => x.Id == counterDto.Id );

    if ( responseCounterDto == null )
      logger.LogError( $"unable to update counter {counterDto.Name}({counterDto.Id}) value. not found in response" );

    responseCounterDto.SetValue( counterDto.Value );

    logger.LogDebug( $"response counter {responseCounterDto.Name}({responseCounterDto.Id}) = {responseCounterDto.Value}" );

  }
}
