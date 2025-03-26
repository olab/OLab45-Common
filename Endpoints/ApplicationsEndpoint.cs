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

public partial class ApplicationsEndpoint : OLabEndpoint
{
  private readonly ApplicationReaderWriter _readerWriter;
  private readonly IOLabMapper<SystemApplications, ApplicationsDto> _mapper;

  public ApplicationsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext) : base(
      logger,
      configuration,
      dbContext )
  {
    _readerWriter = ApplicationReaderWriter.Instance( logger, dbContext );
    _mapper = new ApplicationsMapper(
      GetLogger(),
      GetDbContext() );
  }

  /// <summary>
  /// Get Applications paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<ApplicationsDto>> GetAsync(
    IOLabAuthorization auth,
    int? take, int? skip)
  {
    GetLogger().LogInformation( $"ApplicationsEndpoint.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})" );
    var pagesResult = await _readerWriter.GetAsync( take, skip );

    var pagedDataDto = new OLabAPIPagedResponse<ApplicationsDto>();

    pagedDataDto.Data = _mapper.PhysicalToDto( pagesResult.items.ToList() );
    pagedDataDto.Remaining = pagesResult.remaining;
    pagedDataDto.Count = pagesResult.count;

    return pagedDataDto;

  }

  /// <summary>
  /// Get Group
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>ApplicationsDto or null</returns>
  public async Task<ApplicationsDto> GetAsync(
    IOLabAuthorization auth,
    string source)
  {

    GetLogger().LogInformation( $"ApplicationsEndpoint.ReadAsync(source={source})" );

    var phys = await _readerWriter.GetAsync( source );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "Group", source );

    return _mapper.PhysicalToDto( phys );
  }

  /// <summary>
  /// Create new group
  /// </summary>
  /// <param name="groupName">Group name to create</param>
  /// <returns>Applications</returns>
  public async Task<ApplicationsDto> PostAsync(
    IOLabAuthorization auth,
    string groupName,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"ApplicationsEndpoint.PostAsync()" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
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
    GetLogger().LogInformation( $"ApplicationsEndpoint.DeleteAsync()" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.GetAsync( source );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "Applications", source );

    await _readerWriter.DeleteAsync( source );
  }

}

