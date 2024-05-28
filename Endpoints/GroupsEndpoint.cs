using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class GroupsEndpoint : OLabEndpoint
{
  private readonly GroupRoleReaderWriter _readerWriter;

  public GroupsEndpoint(
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
  }

  /// <summary>
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<Groups>> GetAsync(
    IOLabAuthorization auth,
    int? take, int? skip)
  {
    Logger.LogInformation($"GroupsEndpoint.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");
    return await _readerWriter.GetGroupsPagedAsync(take, skip);
  }

  /// <summary>
  /// Get Group
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns></returns>
  public async Task<Groups> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {

    Logger.LogInformation($"GroupsEndpoint.ReadAsync(uint id={id})");
    return await _readerWriter.GetGroupAsync(id);
  }

  /// <summary>
  /// Create new group
  /// </summary>
  /// <param name="groupName">Group name to create</param>
  /// <returns>Groups</returns>
  public async Task<Groups> PostAsync(
    IOLabAuthorization auth,
    string groupName,
    CancellationToken token)
  {
    Logger.LogInformation($"GroupsEndpoint.PostAsync()");

    // test if user has access 
    if (!await auth.IsSystemSuperuserAsync())
      throw new OLabUnauthorizedException();

    var groupPhys = await _readerWriter.CreateGroupAsync(groupName);

    return groupPhys;
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

