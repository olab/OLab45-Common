using DocumentFormat.OpenXml.Office2010.Excel;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Endpoints.ReaderWriters;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OLab.Api.Endpoints;

public partial class GroupsEndpoint : OLabEndpoint
{

  public GroupsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context) : base(logger, configuration, context)
  {
  }

  /// <summary>
  /// Get group exists
  /// </summary>
  /// <param name="nameOrId">Group name or id</param>
  /// <returns>Group, or null</returns>
  /// 
  private async Task<Groups> GetAsync(uint id)
  {
    return await GetAsync(id.ToString());
  }

  private async Task<Groups> GetAsync(string nameOrId)
  {
    return await GroupsReaderWriter.Instance(Logger, dbContext).GetAsync(nameOrId)
      ?? throw new OLabObjectNotFoundException("Groups", nameOrId);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="nameOrId">Group name or id</param>
  /// <returns></returns>
  public async Task<IdNameDto> GetAsync(
    IOLabAuthorization auth,
    string nameOrId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.GetAsync");

    var phys = await GetAsync(nameOrId)
      ?? throw new OLabObjectNotFoundException("Groups", nameOrId);

    var dto = new ObjectMapper.Groups(Logger).PhysicalToDto(phys);

    return dto;
  }

  /// <summary>
  /// Saves a object edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    IdNameDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.PutAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Groups.GroupNameOLab, Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", dto.Id);

    var phys = await GetAsync(dto.Id)
      ?? throw new OLabObjectNotFoundException("Groups", dto.Id);

    // test if reserved object
    if (Groups.IsReserved(phys.Name))
      throw new OLabUnauthorizedException("Groups", dto.Id);

    try
    {
      var builder = new ObjectMapper.Groups(Logger);
      phys = builder.DtoToPhysical(dto);

      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"GroupsEndpoint.PutAsync {ex.Message}");
    }

  }

  /// <summary>
  /// Create new object
  /// </summary>
  /// <param name="dto">object data</param>
  /// <returns>IActionResult</returns>
  public async Task<IdNameDto> PostAsync(
    IOLabAuthorization auth,
    IdNameDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.PostAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Groups.GroupNameOLab, Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", 0);

    var groupPhys = await GetAsync(dto.Name);
    if (groupPhys != null)
      throw new OLabBadRequestException($"Group '{dto.Name}' already exists");

    // test if reserved object
    if (Groups.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Groups", 0);

    var builder = new ObjectMapper.Groups(Logger);
    groupPhys = builder.DtoToPhysical(dto);

    var transaction = dbContext.Database.BeginTransaction();

    try
    {
      groupPhys = await GroupsReaderWriter
        .Instance(Logger, dbContext).CreateAsync(auth, groupPhys);

      await dbContext.SaveChangesAsync();
      transaction.Commit();
    }
    catch (Exception ex)
    {
      Logger.LogError($"GroupsEndpoint.DeleteAsync {ex.Message}");
      await transaction.RollbackAsync();
    }


    await dbContext.SaveChangesAsync();

    dto = builder.PhysicalToDto(groupPhys);

    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    string nameOrId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.DeleteAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Groups.GroupNameOLab, Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", nameOrId);

    var groupPhys = await GetAsync(nameOrId)
      ?? throw new OLabObjectNotFoundException("Groups", nameOrId);

    if (Groups.IsReserved(groupPhys.Name))
      throw new OLabUnauthorizedException("Groups", nameOrId);

    var transaction = dbContext.Database.BeginTransaction();

    try
    {
      // delete all the related group references
      await GroupsReaderWriter
        .Instance(Logger, dbContext).DeleteAsync(groupPhys);

      await dbContext.SaveChangesAsync();
      transaction.Commit();
    }
    catch (Exception ex)
    {
      Logger.LogError($"GroupsEndpoint.DeleteAsync {ex.Message}");
      await transaction.RollbackAsync();
    }

  }

  public async Task<PagedResult<Groups>> GetAsync(
    IOLabAuthorization auth, 
    int? take, 
    int? skip)
  {
    var result = await GroupsReaderWriter
      .Instance(Logger, dbContext).GetAsync(auth, take, skip);
    return result;
  }
}
