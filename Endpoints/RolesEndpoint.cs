using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Data.Mappers;
using OLab.Data.ReaderWriters;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class RolesEndpoint : OLabEndpoint
{
  private readonly RoleReaderWriter _readerWriter;
  private readonly IOLabMapper<Roles, RolesDto> _mapper;

  public RolesEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      dbContext,
      wikiTagProvider,
      fileStorageProvider )
  {
    _readerWriter = RoleReaderWriter.Instance( logger, dbContext );
    _mapper = new RolesMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
  }

  /// <summary>
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<RolesDto>> GetAsync(
    IOLabAuthorization auth,
    int? take, int? skip)
  {
    var physItems = await _readerWriter.GetAsync<Roles>( skip, take );

    var dtoItems = new OLabAPIPagedResponse<RolesDto>();
    dtoItems.Data = _mapper.PhysicalToDto( physItems.items );
    dtoItems.Remaining = physItems.remaining;
    dtoItems.Count = physItems.count;

    return dtoItems;

  }

  /// <summary>
  /// Get Role
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns></returns>
  public async Task<RolesDto> GetAsync(
    IOLabAuthorization auth,
    string source)
  {

    GetLogger().LogInformation( $"RolesEndpoint.ReadAsync(source={source})" );
    var phys = await _readerWriter.GetAsync( source );

    return _mapper.PhysicalToDto( phys );
  }

  /// <summary>
  /// Create new group
  /// </summary>
  /// <param name="groupName">Role name to create</param>
  /// <returns>Roles</returns>
  public async Task<RolesDto> PostAsync(
    IOLabAuthorization auth,
    string groupName,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"RolesEndpoint.PostAsync()" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    // test for reserved object
    var orgPhys = await _readerWriter.GetAsync( groupName );
    if ( (orgPhys != null) && (orgPhys.IsSystem == 1) )
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.CreateAsync( groupName );
    return _mapper.PhysicalToDto( phys );
  }

  /// <summary>
  /// Deletes a group
  /// </summary>
  /// <param name="id">Role id to delete</param>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    string source)
  {
    {
      GetLogger().LogInformation( $"RolesEndpoint.DeleteAsync()" );

      // test if user has access 
      if ( !await auth.IsSystemSuperuserAsync() )
        throw new OLabUnauthorizedException();

      var phys = await _readerWriter.GetAsync( source );
      if ( phys == null )
        throw new OLabObjectNotFoundException( "Roles", source );

      // test if reserved object
      if ( (phys != null) && (phys.IsSystem == 1) )
        throw new OLabUnauthorizedException();

      // test if in use somewhere
      var inUse = await GetDbContext().UserGrouproles.AnyAsync( x => x.RoleId == phys.Id );
      if ( inUse )
        throw new OLabGeneralException( $"Role '{source}' in use." );

      await _readerWriter.DeleteAsync( source );
    }
  }

}

