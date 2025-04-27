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
  private readonly IDictionary<uint, SystemConstants> _constantIds = new Dictionary<uint, SystemConstants>();
  private readonly IDictionary<string, SystemConstants> _constantNames = new Dictionary<string, SystemConstants>();
  
  public void AddConstantCrossReference(SystemConstants from, SystemConstants to) 
  { 
    _constantIds.Add( from.Id, to );
    _constantNames.Add( from.Name, to ); 
  }

  public string GetConstantCrossReference(string id)
  {
    if ( UInt32.TryParse( id, out uint constantId ) )
    {
      if ( _constantIds.ContainsKey( constantId ) )
        return _constantIds[ constantId ].Name;
    }
    else if ( _constantNames.ContainsKey( id ) )
      return _constantNames[ id ].Name;

    throw new KeyNotFoundException( $"Constant '{id}' not found" );
  }

}
