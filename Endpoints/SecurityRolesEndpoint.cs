using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Endpoints.ReaderWriters;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class SecurityRolesEndpoint : OLabEndpoint
{

  public SecurityRolesEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context) : base(logger, configuration, context)
  {
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  private bool Exists(uint id)
  {
    return dbContext.SecurityRoles.Any(e => e.Id == id);
  }

  /// <summary>
  /// PAged get of Groups
  /// </summary>
  /// <param name="take">Max number of records to retrieve</param>
  /// <param name="skip">Skip over number of records</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<SecurityRolesDto>> GetAsync(
    IOLabAuthorization auth,
    int? take,
    int? skip)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.ReadAsync");

    var securityRolesPhys = new List<Model.SecurityRoles>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    total = dbContext.SecurityRoles.Count();

    if (take.HasValue && skip.HasValue)
    {
      securityRolesPhys = await dbContext.SecurityRoles.Skip(skip.Value).Take(take.Value).ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      securityRolesPhys = await dbContext.SecurityRoles.ToListAsync();
      remaining = 0;
    }

    var dtoList = new ObjectMapper.SecurityRoles(Logger).PhysicalToDto(securityRolesPhys);

    Logger.LogInformation(string.Format("found {0} roles", dtoList.Count));

    return new OLabAPIPagedResponse<SecurityRolesDto> { Data = dtoList, Remaining = remaining, Count = total };
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<SecurityRolesDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.ReadAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("SecurityRoles", id);

    var phys = await dbContext.SecurityRoles.FirstAsync(x => x.Id == id);
    var dto = new ObjectMapper.SecurityRoles(Logger).PhysicalToDto(phys);

    return dto;
  }

  /// <summary>
  /// Saves a object edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id,
    SecurityRolesDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.PutAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("SecurityRoles", id);

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("SecurityRoles", id);

    try
    {
      var builder = new ObjectMapper.SecurityRoles(Logger);
      var phys = builder.DtoToPhysical(dto);

      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"SecurityRolesEndpoint.PutAsync {ex.Message}");
    }

  }

  /// <summary>
  /// Create new object
  /// </summary>
  /// <param name="dto">object data</param>
  /// <returns>IActionResult</returns>
  public async Task<SecurityRolesDto> PostAsync(
    IOLabAuthorization auth,
    SecurityRolesDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.PostAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("SecurityRoles", 0);

    var builder = new ObjectMapper.SecurityRoles(Logger);
    var phys = builder.DtoToPhysical(dto);

    dbContext.SecurityRoles.Add(phys);
    await dbContext.SaveChangesAsync();

    dto = builder.PhysicalToDto(phys);

    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.DeleteAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("SecurityRoles", id);

    if (!Exists(id))
      throw new OLabObjectNotFoundException("SecurityRoles", id);

    var phys = await dbContext.SecurityRoles.FirstOrDefaultAsync(x => x.Id == id);

    try
    {
      dbContext.SecurityRoles.Remove(phys);
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"SecurityRolesEndpoint.DeleteAsync {ex.Message}");
    }

  }

  /// <summary>
  /// Get records for a given group and role
  /// </summary>
  /// <param name="auth">IOLabAuthorization</param>
  /// <param name="groupIdName">Group id/name</param>
  /// <param name="roleIdName">Role id/name</param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>

  public async Task<IList<SecurityRolesDto>> GetGroupRoleAsync(
    IOLabAuthorization auth, 
    string groupIdName, 
    string roleIdName = null)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: SecurityRolesEndpoint.DeleteAsync");

    IList<Model.SecurityRoles> physList;

    var groupPhys = await GroupsReaderWriter.Instance(Logger, dbContext).GetAsync(groupIdName);
    if (!string.IsNullOrEmpty(roleIdName))
    {
      var rolePhys = await RolesReaderWriter.Instance(Logger, dbContext).GetAsync(roleIdName);
      physList = dbContext.SecurityRoles.Where(x => x.GroupId == groupPhys.Id && x.RoleId == rolePhys.Id ).ToList();
    }
    else
      physList = dbContext.SecurityRoles.Where(x => x.GroupId == groupPhys.Id ).ToList();

    var dtoList = new ObjectMapper.SecurityRoles(Logger).PhysicalToDto(physList);

    return dtoList;
  }
}
