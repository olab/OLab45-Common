using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OLab.Common.Utils;
public class SerializerUtilities
{

  public static string TruncateJsonToDepth(string jsonString, int maxDepth)
  {
    using var doc = JsonDocument.Parse( jsonString );
    return JsonSerializer.Serialize( TruncateElement( doc.RootElement, maxDepth + 2 ) );
  }

  private static JsonElement TruncateElement(JsonElement element, int maxDepth)
  {
    if ( maxDepth <= 0 )
    {
      return JsonDocument.Parse( "null" ).RootElement;
    }

    switch ( element.ValueKind )
    {
      case JsonValueKind.Object:
        return TruncateObject( element, maxDepth );
      case JsonValueKind.Array:
        return TruncateArray( element, maxDepth );
      default:
        return element.Clone();
    }
  }

  private static JsonElement TruncateObject(JsonElement obj, int maxDepth)
  {
    var truncated = new Dictionary<string, JsonElement>();
    foreach ( var property in obj.EnumerateObject() )
    {
      if ( maxDepth > 1 )
      {
        truncated[ property.Name ] = TruncateElement( property.Value, maxDepth - 1 );
      }
    }
    return JsonSerializer.Deserialize<JsonElement>( JsonSerializer.Serialize( truncated ) );
  }

  private static JsonElement TruncateArray(JsonElement arr, int maxDepth)
  {
    if ( maxDepth <= 1 )
    {
      return JsonSerializer.Deserialize<JsonElement>( "[]" );
    }

    var truncated = arr.EnumerateArray()
        .Select( item => TruncateElement( item, maxDepth - 1 ) )
        .ToList();
    return JsonSerializer.Deserialize<JsonElement>( JsonSerializer.Serialize( truncated ) );
  }



}
