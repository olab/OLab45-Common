using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Data.Mappers;
using OLab.Data.ReaderWriters;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class GroupsEndpoint : OLabEndpoint
{
  private readonly GroupReaderWriter _readerWriter;
  private readonly GroupsMapper _mapper;

  public GroupsEndpoint(
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
    _readerWriter = GroupReaderWriter.Instance( logger, dbContext );
    _mapper = new GroupsMapper( GetLogger(),
      GetDbContext(),
      GetWikiProvider() );
  }

  /// <summary>
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<GroupsDto>> GetAsync(
    IOLabAuthorization auth,
    int? take, int? skip)
  {
    GetLogger().LogInformation( $"GroupsEndpoint.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})" );
    var pagedDataPhys = await _readerWriter.GetPagedAsync( take, skip );

    var pagedDataDto = new OLabAPIPagedResponse<GroupsDto>();

    pagedDataDto.Data = _mapper.PhysicalToDto( pagedDataPhys.Data );
    pagedDataDto.Remaining = pagedDataPhys.Remaining;
    pagedDataDto.Count = pagedDataPhys.Count;

    return pagedDataDto;

  }

  /// <summary>
  /// Get Group
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>GroupsDto or null</returns>
  public async Task<GroupsDto> GetAsync(
    IOLabAuthorization auth,
    string source)
  {

    GetLogger().LogInformation( $"GroupsEndpoint.ReadAsync(source={source})" );

    var phys = await _readerWriter.GetAsync( source );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "Group", source );

    return _mapper.PhysicalToDto( phys );
  }

  /// <summary>
  /// Create new group
  /// </summary>
  /// <param name="groupName">Group name to create</param>
  /// <returns>Groups</returns>
  public async Task<GroupsDto> PostAsync(
    IOLabAuthorization auth,
    string groupName,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"GroupsEndpoint.PostAsync()" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    // test for reserved object
    var orgPhys = await _readerWriter.GetAsync( groupName );
    if ( (orgPhys != null) && (orgPhys.IsSystem == 1) )
      throw new OLabUnauthorizedException();

    // test 
    var phys = await _readerWriter.CreateAsync( groupName );
    return _mapper.PhysicalToDto( phys );
  }

  /// <summary>
  /// Deletes a group
  /// </summary>
  /// <param name="id">Group id to delete</param>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    string source)
  {
    GetLogger().LogInformation( $"GroupsEndpoint.DeleteAsync()" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.GetAsync( source );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "Groups", source );

    // test if reserved object
    if ( (phys != null) && (phys.IsSystem == 1) )
      throw new OLabUnauthorizedException();

    // test if in use somewhere
    var inUse = await GetDbContext().MapGrouproles.AnyAsync( x => x.GroupId == phys.Id ) ||
                await GetDbContext().UserGrouproles.AnyAsync( x => x.GroupId == phys.Id );
    if ( inUse )
      throw new OLabGeneralException( $"Group '{source}' in use." );

    await _readerWriter.DeleteAsync( source );
  }

}

