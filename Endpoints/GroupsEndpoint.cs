using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
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
  private readonly GroupRoleReaderWriter _readerWriter;
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
      fileStorageProvider)
  {
    _readerWriter = GroupRoleReaderWriter.Instance(logger, dbContext);
    _mapper = new GroupsMapper(logger);
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
    Logger.LogInformation($"GroupsEndpoint.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");
    var pagedDataPhys = await _readerWriter.GetGroupsPagedAsync(take, skip);

    var pagedDataDto = new OLabAPIPagedResponse<GroupsDto>();

    pagedDataDto.Data = _mapper.PhysicalToDto(pagedDataPhys.Data);
    pagedDataDto.Remaining = pagedDataPhys.Remaining;
    pagedDataDto.Count = pagedDataPhys.Count;

    return pagedDataDto;

  }

  /// <summary>
  /// Get Group
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns></returns>
  public async Task<GroupsDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {

    Logger.LogInformation($"GroupsEndpoint.ReadAsync(uint id={id})");
    var phys = await _readerWriter.GetGroupAsync(id);

    return _mapper.PhysicalToDto(phys);
  }

  /// <summary>
  /// Get Group
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns></returns>
  public async Task<GroupsDto> GetAsync(
    IOLabAuthorization auth,
    string name)
  {

    Logger.LogInformation($"GroupsEndpoint.ReadAsync(uint name={name})");
    var phys = await _readerWriter.GetGroupAsync(name);

    return _mapper.PhysicalToDto(phys);
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
    Logger.LogInformation($"GroupsEndpoint.PostAsync()");

    // test if user has access 
    if (!await auth.IsSystemSuperuserAsync())
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.CreateGroupAsync(groupName);
    return _mapper.PhysicalToDto(phys);
  }

  /// <summary>
  /// Deletes a group
  /// </summary>
  /// <param name="id">Group id to delete</param>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"GroupsEndpoint.DeleteAsync()");

    // test if user has access 
    if (!await auth.IsSystemSuperuserAsync())
      throw new OLabUnauthorizedException();

    await _readerWriter.DeleteGroupAsync(id);
  }

}

