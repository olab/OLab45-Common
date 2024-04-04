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

public partial class GroupsEndpoint : OLabEndpoint
{

  public GroupsEndpoint(
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
    return dbContext.Groups.Any(e => e.Id == id);
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
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.ReadAsync");

    var groupsPhys = new List<Model.Groups>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    total = dbContext.Groups.Count();

    if (take.HasValue && skip.HasValue)
    {
      groupsPhys = await dbContext.Groups.Skip(skip.Value).Take(take.Value).ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      groupsPhys = await dbContext.Groups.OrderBy(x => x.Name).ToListAsync();
      remaining = 0;
    }

    var dtoList = new ObjectMapper.Groups(Logger).PhysicalToDto(groupsPhys);

    Logger.LogInformation(string.Format("found {0} groups", dtoList.Count));

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
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.ReadAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Groups", id);

    var phys = await dbContext.Groups.FirstAsync(x => x.Id == id);
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
    uint id,
    IdNameDto dto)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.PutAsync");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Groups", id);

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", id);

    // test if reserved object
    if (Model.Groups.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Groups", id);

    try
    {
      var builder = new ObjectMapper.Groups(Logger);
      var phys = builder.DtoToPhysical(dto);

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
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", 0);

    // test if reserved object
    if (Model.Groups.IsReserved(dto.Name))
      throw new OLabUnauthorizedException("Groups", 0);

    var builder = new ObjectMapper.Groups(Logger);
    var phys = builder.DtoToPhysical(dto);

    dbContext.Groups.Add(phys);
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
    Logger.LogInformation($"{auth.UserContext.UserId}: GroupsEndpoint.DeleteAsync");

    // test if user has access to object
    if (!auth.IsMemberOf(Model.Groups.GroupNameOLab, Model.Roles.RoleNameSuperuser))
      throw new OLabUnauthorizedException("Groups", id);

    if (!Exists(id))
      throw new OLabObjectNotFoundException("Groups", id);

    var phys = await dbContext.Groups.FirstOrDefaultAsync(x => x.Id == id);

    if (Model.Groups.IsReserved(phys.Name))
      throw new OLabUnauthorizedException("Groups", id);

    try
    {
      dbContext.Groups.Remove(phys);
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      Logger.LogError($"GroupsEndpoint.DeleteAsync {ex.Message}");
    }

  }

}
