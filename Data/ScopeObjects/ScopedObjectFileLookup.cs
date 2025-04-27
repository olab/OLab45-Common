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
  private readonly IDictionary<uint, SystemFiles> _fileIds = new Dictionary<uint, SystemFiles>();
  private readonly IDictionary<string, SystemFiles> _fileNames = new Dictionary<string, SystemFiles>();
  
  public void AddFileCrossReference(SystemFiles from, SystemFiles to) 
  { 
    _fileIds.Add( from.Id, to );
    _fileNames.Add( from.Name, to ); 
  }

  public string GetFileCrossReference(string id)
  {
    if ( UInt32.TryParse( id, out uint constantId ) )
    {
      if ( _fileIds.ContainsKey( constantId ) )
        return _fileIds[ constantId ].Name;
    }
    else if ( _fileNames.ContainsKey( id ) )
      return _fileNames[ id ].Name;

    throw new KeyNotFoundException( $"File '{id}' not found" );
  }

  private string GetFileIdCrossReference(string id) 
  {
    if ( _fileNames.ContainsKey( id ) )
      return _fileNames[ id ].Name;
    return null;
  }

  private string GetFileIdCrossReference(uint id)
  {
    if ( _fileIds.ContainsKey( id ) )
      return _fileIds[ id ].Name;
    return null;
  }


}
