using Humanizer;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  private bool Exists(uint id)
  {
    return dbContext.Roles.Any(e => e.Id == id);
  }

  /// <summary>
  /// PAged get of Groups
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

    Logger.LogInformation(string.Format("found {0} roles", dtoList.Count));

    return new OLabAPIPagedResponse<IdNameDto> { Data = dtoList, Remaining = remaining, Count = total };
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<IdNameDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.ReadAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Roles", id);

    var phys = await dbContext.Roles.FirstAsync(x => x.Id == id);
    var dto = new ObjectMapper.Roles(Logger).PhysicalToDto(phys);

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
    IdNameDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.PutAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Roles", id);

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Roles", id);

    // test if reserved object
    if (Model.Roles.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Roles", id);

    try
    {
      var builder = new ObjectMapper.Roles(Logger);
      var phys = builder.DtoToPhysical(dto);

      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"RolesEndpoint.PutAsync {ex.Message}");
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

    // test if reserved object
    if (Model.Roles.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Roles", 0);

    var builder = new ObjectMapper.Roles(Logger);
    var phys = builder.DtoToPhysical(dto);

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
    uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: RolesEndpoint.DeleteAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Roles", id);

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Roles", id);

    var phys = await dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id);

    if (Model.Roles.IsReserved(phys.Name))
      throw new OLabUnauthorizedException("Roles", id);

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
