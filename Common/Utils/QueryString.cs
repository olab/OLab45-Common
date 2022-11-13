using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Utils
{
  public class QueryStringUtils
  {
    protected IDictionary<string, string> _values = new Dictionary<string, string>();

    public QueryStringUtils(HttpRequest req)
    {
      foreach (var item in req.Query)
      {
        _values.Add(item.Key, item.Value);
      }
    }

    public string Get(string key)
    {
      if (!_values.TryGetValue(key, out string value))
        throw new KeyNotFoundException(key);

      return value;
    }

    public int? GetOptional(string key, int? defaultValue )
    {
      if (!_values.TryGetValue(key, out string value))
        return defaultValue;

      return Convert.ToInt32(value);
    }

    public string GetOptional(string key, string defaultValue )
    {
      if (!_values.TryGetValue(key, out string value))
        return defaultValue;
      return value;
    }    
  }
}