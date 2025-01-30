using Dawn;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace OLab.Api.ObjectMapper;

public abstract class ObjectMapper<P, D> : object where P : new() where D : new()
{
  protected IOLabLogger _logger;
  protected IOLabLogger GetLogger() { return _logger; }

  private readonly OLabDBContext _dbContext;
  protected OLabDBContext GetDbContext() { return _dbContext; }

  protected IOLabModuleProvider<IWikiTagModule> _wikiTagModules = null;
  public WikiTagModuleProvider GetWikiProvider() { return _wikiTagModules as WikiTagModuleProvider; }

  // used to hold on to id translation between origin system and new one
  protected IDictionary<uint, uint?> _idTranslation = new Dictionary<uint, uint?>();

  public abstract D PhysicalToDto(P phys, Object source = null);
  public abstract P DtoToPhysical(D dto, Object source = null);
  public virtual P ElementsToPhys(IEnumerable<dynamic> elements, Object source = null) { return default; }

  public ObjectMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider = null)
  {
    Guard.Argument( logger ).NotNull( nameof( logger ) );
    Guard.Argument( dbContext ).NotNull( nameof( dbContext ) );

    _logger = OLabLogger.CreateNew<OLabMapper<P, D>>( logger );
    _dbContext = dbContext;
    _wikiTagModules = wikiTagProvider as WikiTagModuleProvider;
  }

  public virtual D GetDto(Object source = null)
  {
    if ( source == null )
      return new D();

    return (D)source;
  }

  public virtual P GetPhys(Object source = null)
  {
    if ( source == null )
      return new P();

    return (P)source;
  }

  public virtual IList<D> PhysicalToDto(IList<P> physList)
  {
    var dtoList = new List<D>();
    foreach ( var phys in physList )
    {
      var dto = PhysicalToDto( phys );
      dtoList.Add( dto );
    }

    return dtoList;
  }

  protected void CreateIdTranslation(uint originalId)
  {
    if ( _idTranslation.ContainsKey( originalId ) )
      return;
    _idTranslation.Add( originalId, null );
  }

  protected bool SetIdTranslation(uint originalId, uint newId)
  {
    return _idTranslation.TryAdd( originalId, newId );
  }

  protected uint? GetIdTranslation(uint originalId)
  {
    if ( !_idTranslation.TryGetValue( originalId, out var newId ) )
      return newId;

    throw new KeyNotFoundException( $"Cound not find Id key {originalId}" );
  }

}