using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

public class QueryStrings 
{
  private Dictionary<string, string> _values;

  public QueryStrings(HttpRequest request)
  {
    _values = new Dictionary<string, string>();
    foreach (var item in request.Query)
      _values.Add( item.Key, item.Value );
  }

  public T Get<T>( string key, T defaultValue )
  {
    if ( _values.TryGetValue( key, out string value) )
      return (T) Convert.ChangeType( value, typeof(T) );
    return defaultValue;
  }

}

public class QueryStringHelper
{
  public static QueryStrings Instance(HttpRequest request) 
  {
    return new QueryStrings( request );
  }

}