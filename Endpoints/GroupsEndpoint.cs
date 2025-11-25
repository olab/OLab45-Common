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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class GroupsEndpoint : OLabEndpoint
{
  private readonly GroupReaderWriter _readerWriter;
  private readonly IOLabMapper<Groups, GroupsDto> _mapper;

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
    _mapper = new GroupsMapper(
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
  public async Task<OLabAPIPagedResponse<GroupsDto>> GetAsync(
    IOLabAuthorization auth,
    int? take, int? skip)
  {
    var physItems = await _readerWriter.GetRawAsync<Groups>();
    var total = physItems.count;

    var dtoResponse = new OLabAPIPagedResponse<GroupsDto>();

    // if not superuser, then build list of groups user is part of
    if ( !await auth.IsSystemSuperuserAsync() )
    {
      physItems.items
        = physItems.items.Where( x => auth.UsersGroupRoles.Select( y => y.Id ).ToList().Contains( x.Id ) ).Distinct().ToList();
      total = physItems.items.Count();
    }

    if ( take.HasValue )
      physItems.items = physItems.items.Skip( skip.Value ).Take( take.Value ).ToList();

    dtoResponse.Data = _mapper.PhysicalToDto( physItems.items.OrderBy( x => x.Name ).ToList() );
    dtoResponse.Remaining = total - physItems.items.Count();
    dtoResponse.Count = physItems.items.Count();

    return dtoResponse;
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
    if ( (orgPhys != null) && orgPhys.IsSystem )
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
    if ( (phys != null) && phys.IsSystem )
      throw new OLabUnauthorizedException();

    // test if in use somewhere
    var inUse = await GetDbContext().MapGrouproles.AnyAsync( x => x.GroupId == phys.Id ) ||
                await GetDbContext().UserGrouproles.AnyAsync( x => x.GroupId == phys.Id );
    if ( inUse )
      throw new OLabGeneralException( $"Group '{source}' in use." );

    await _readerWriter.DeleteAsync( source );
  }

}

