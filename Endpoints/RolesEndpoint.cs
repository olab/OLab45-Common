using DocumentFormat.OpenXml.Office2010.Excel;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Endpoints.ReaderWriters;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OLab.Api.Endpoints;

public partial class RolesEndpoint : OLabEndpoint
{

  public RolesEndpoint(
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
  private async Task<Model.Roles> GetAsync(uint id)
  {
    return await GetAsync(id.ToString());
  }

  private async Task<Model.Roles> GetAsync(string nameOrId, bool throwIfNotFound = false)
  {
    var phys = await RolesReaderWriter
      .Instance(Logger, dbContext).GetAsync(nameOrId, throwIfNotFound);
    return phys;
  }

  /// <summary>
  /// Paged reading of Roles
  /// </summary>
  /// <param name="take">Max number of records to retrieve</param>
  /// <param name="skip">Skip over number of records</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<IdNameDto>> GetAsync(
    IOLabAuthorization auth,
    int? take,
    int? skip)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.ReadAsync");

    var groupsPhys = new List<Model.Roles>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    total = dbContext.Roles.Count();

    if (take.HasValue && skip.HasValue)
    {
      groupsPhys = await dbContext.Roles.Skip(skip.Value).Take(take.Value).ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      groupsPhys = await dbContext.Roles.OrderBy(x => x.Name).ToListAsync();
      remaining = 0;
    }

    var dtoList = new ObjectMapper.Roles(Logger).PhysicalToDto(groupsPhys);

    Logger.LogInformation(string.Format("found {0} groups", dtoList.Count));

    return new OLabAPIPagedResponse<IdNameDto> { Data = dtoList, Remaining = remaining, Count = total };
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
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.GetAsync");

    var phys = await GetAsync(nameOrId);
    if (phys == null)
      throw new OLabObjectNotFoundException("Roles", nameOrId);

    var dto = new ObjectMapper.Roles(Logger).PhysicalToDto(phys);

    return dto;
  }

  /// Saves a object edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    IdNameDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolessEndpoint.PutAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Roles", dto.Id);

    var phys = await GetAsync(dto.Id);
    if (phys == null)
      throw new OLabObjectNotFoundException("Roles", dto.Id);

    // test if reserved object
    if (Model.Roles.IsReserved(phys.Name))
      throw new OLabUnauthorizedException("Roles", dto.Id);

    if (Model.Roles.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Roles", dto.Name);

    try
    {
      var builder = new ObjectMapper.Roles(Logger);
      phys = builder.DtoToPhysical(dto);

      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"RolessEndpoint.PutAsync {ex.Message}");
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
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.PostAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Roles", 0);

    var phys = await GetAsync(dto.Name);
    if (phys != null)
      throw new OLabInvalidRequestException($"Role '{dto.Name}' already exists");

    // test if reserved object
    if (Model.Roles.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Roles", 0);

    var builder = new ObjectMapper.Roles(Logger);
    phys = builder.DtoToPhysical(dto);

    dbContext.Roles.Add(phys);
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
    string nameOrId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.DeleteAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Roles", nameOrId);

    var phys = await GetAsync(nameOrId);
    if (phys == null)
      throw new OLabObjectNotFoundException("Roles", nameOrId);

    if (Model.Roles.IsReserved(phys.Name))
      throw new OLabUnauthorizedException("Roles", nameOrId);

    try
    {
      dbContext.Roles.Remove(phys);
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"RolesEndpoint.DeleteAsync {ex.Message}");
    }

  }

}
