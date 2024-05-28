using Microsoft.EntityFrameworkCore;
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

public partial class RolesEndpoint : OLabEndpoint
{
  private readonly GroupRoleReaderWriter _readerWriter;
  private readonly RolesMapper _mapper;

  public RolesEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageProvider)
  {
    _readerWriter = GroupRoleReaderWriter.Instance(logger, context);
    _mapper = new RolesMapper(logger);
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
    Logger.LogInformation($"RolesEndpoint.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");
    var pagedDataPhys = await _readerWriter.GetRolesPagedAsync(take, skip);

    var pagedDataDto = new OLabAPIPagedResponse<RolesDto>();

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
  public async Task<RolesDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {

    Logger.LogInformation($"RolesEndpoint.ReadAsync(uint id={id})");
    var phys = await _readerWriter.GetRoleAsync(id);

    return _mapper.PhysicalToDto(phys);

  }

  /// <summary>
  /// Create new group
  /// </summary>
  /// <param name="groupName">Group name to create</param>
  /// <returns>Roles</returns>
  public async Task<RolesDto> PostAsync(
    IOLabAuthorization auth,
    string groupName,
    CancellationToken token)
  {
    Logger.LogInformation($"RolesEndpoint.PostAsync()");

    // test if user has access 
    if (!await auth.IsSystemSuperuserAsync())
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.CreateRoleAsync(groupName);
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
    Logger.LogInformation($"RolesEndpoint.DeleteAsync()");

    // test if user has access 
    if (!await auth.IsSystemSuperuserAsync())
      throw new OLabUnauthorizedException();

    await _readerWriter.DeleteRoleAsync(id);
  }

}

