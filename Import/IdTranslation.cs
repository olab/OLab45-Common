using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Import;
public class IdTranslation
{
  private IDictionary<uint, uint?> _idTranslation = new Dictionary<uint, uint?>();
  private IOLabLogger _logger;
  private string _containerName;

  public IdTranslation(IOLabLogger logger, string containerName)
  {
    _logger = logger;
    _containerName = containerName;
  }

  /// <summary>
  /// Add id translation record to store
  /// </summary>
  /// <param name="originalId">Import system Id</param>
  /// <param name="newId">Database id</param>
  public virtual bool CreateIdTranslation(uint originalId, uint? newId)
  {
    if ( _idTranslation.ContainsKey( originalId ) )
    {
      _logger.LogInformation( $"  replaced {_containerName} translation id {originalId} -> {newId.Value}" );
      _idTranslation[ originalId ] = newId;
      return false;
    }

    _idTranslation.Add( originalId, newId );
    _logger.LogInformation( $"  added {_containerName} translation id {originalId} -> {newId.Value}" );

    return true;
  }

  /// <summary>
  /// ReadAsync postimport object creation id
  /// </summary>
  /// <param name="originalId">Import system id</param>
  /// <returns></returns>
  public virtual uint? GetIdTranslation(string referencedFile, uint originalId)
  {
    if ( originalId == 0 )
      return 0;

    if ( _idTranslation.TryGetValue( originalId, out var newId ) )
      return newId;

    throw new KeyNotFoundException( $"references {_containerName} Id {originalId}: not found" );
  }

  /// <summary>
  /// ReadAsync postimport object creation id
  /// </summary>
  /// <param name="originalId">(nullable) original id</param>
  /// <returns></returns>
  public virtual int? GetIdTranslation(string referencedFile, int? originalId)
  {
    if ( !originalId.HasValue )
      return originalId;
    var value = GetIdTranslation( referencedFile, (uint)originalId.Value );
    return (int?)value;
  }

}
