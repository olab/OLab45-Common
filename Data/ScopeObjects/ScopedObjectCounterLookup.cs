using Humanizer;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Data;

public partial class ScopedObjects
{
  private readonly IDictionary<uint, SystemCounters> _counterIds = new Dictionary<uint, SystemCounters>();
  private readonly IDictionary<string, SystemCounters> _counterNames = new Dictionary<string, SystemCounters>();
  
  public void AddCounterCrossReference(SystemCounters from, SystemCounters to) 
  { 
    _counterIds.Add( from.Id, to );
    _counterNames.Add( from.Name, to ); 
  }

  public string GetCounterCrossReference(string id)
  {
    if ( UInt32.TryParse( id, out uint CounterId ) )
    {
      if ( _counterIds.ContainsKey( CounterId ) )
        return _counterIds[ CounterId ].Name;
    }
    else if ( _counterNames.ContainsKey( id ) )
      return _counterNames[ id ].Name;

    throw new KeyNotFoundException( $"Counter '{id}' not found" );
  }

}
