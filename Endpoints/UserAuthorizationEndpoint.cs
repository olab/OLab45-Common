
using DocumentFormat.OpenXml.InkML;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class UserAuthorizationEndpoint : OLabEndpoint
{
  private readonly UserGroupRolesMapper mapper;

  public UserAuthorizationEndpoint(
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
    mapper = new UserGroupRolesMapper(Logger, dbContext);
  }

  /// <summary>
  /// Remove group/role from user
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">Map group to remove</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All group/roles for user</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<UserGroupRolesDto>> DeleteAsync(
    IOLabAuthorization auth,
    UserGroupRolesDto dto,
    CancellationToken token)
  {
    Logger.LogInformation($"UserAuthorizationEndpoint.DeleteAsync()");

    // test if user has access to parent object
    var accessResult = auth.HasAccess(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0);

    if (!accessResult)
      throw new OLabUnauthorizedException("User", dto.UserId);

    var userPhys = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId, token);
    if (userPhys == null)
      throw new OLabObjectNotFoundException("Users", dto.UserId);

    var mapGroupPhys = userPhys.UserGrouproles
      .FirstOrDefault(x => (x.GroupId == dto.GroupId) && (x.RoleId == dto.RoleId));

    if (mapGroupPhys != null)
    {
      userPhys.UserGrouproles.Remove(mapGroupPhys);
      dbContext.SaveChanges();
    }
    else
      throw new OLabObjectNotFoundException("UserGroupRole", 0);

    return mapper.PhysicalToDto(userPhys.UserGrouproles.ToList());

  }

  /// <summary>
  /// Add group/role to a user
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">MapGroupsDto</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All groups for map</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<UserGroupRolesDto>> AddAsync(
    IOLabAuthorization auth,
    UserGroupRolesDto dto,
    CancellationToken token)

  {
    Logger.LogInformation($"UserAuthorizationEndpoint.AddAsync()");

    // test if user has access to parent object
    var accessResult = auth.HasAccess(
      IOLabAuthorization.AclBitMaskExecute,
      "UserAdmin",
      0);

    if (!accessResult)
      throw new OLabUnauthorizedException("User", dto.UserId);

    var userPhys = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId, token);
    if (userPhys == null)
      throw new OLabObjectNotFoundException("Users", dto.UserId);

    var reader = GroupRoleReaderWriter.Instance(Logger, dbContext);

    // ensure group exists
    if (await reader.GroupExistsAsync(dto.GroupId))
      throw new OLabObjectNotFoundException("Group", dto.GroupId);

    // ensure role exists
    if (await reader.RoleExistsAsync(dto.RoleId))
      throw new OLabObjectNotFoundException("Role", dto.RoleId);

    // test if doesn't already exist
    if (!userPhys.UserGrouproles.Any(x => (x.RoleId == dto.RoleId) && (x.GroupId == dto.GroupId)))
    {
      var userGroupRolePhys = mapper.DtoToPhysical(dto);
      userPhys.UserGrouproles.Add(userGroupRolePhys);

      dbContext.SaveChanges();
    }

    return mapper.PhysicalToDto(userPhys.UserGrouproles.ToList());

  }

}
