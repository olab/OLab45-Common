using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class CountersEndpoint : OLabEndpoint
{

  public CountersEndpoint(
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
    return GetDbContext().SystemCounters.Any(e => e.Id == id);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="take"></param>
  /// <param name="skip"></param>
  /// <returns></returns>
  public async Task<OLabAPIPagedResponse<CountersDto>> GetAsync(
    IOLabAuthorization auth,
    int? take,
    int? skip)
  {
    GetLogger().LogInformation($"ReadAsync take={take} skip={skip}");

    var countersPhys = new List<SystemCounters>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    countersPhys = await GetDbContext().SystemCounters.OrderBy(x => x.Name).ToListAsync();
    if (countersPhys.Count == 0)
      return new OLabAPIPagedResponse<CountersDto> { Data = new List<CountersDto>(), Remaining = 0, Count = 0 };

    total = countersPhys.Count;

    if (take.HasValue && skip.HasValue)
    {
      countersPhys = countersPhys.Skip(skip.Value).Take(take.Value).ToList();
      remaining = total - take.Value - skip.Value;
    }

    var dtoList = new ObjectMapper.CounterMapper(GetLogger()).PhysicalToDto(countersPhys);

    var maps = GetDbContext().Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
    var nodes = GetDbContext().MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
    var servers = GetDbContext().Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

    foreach (var dto in dtoList)
      dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

    return new OLabAPIPagedResponse<CountersDto> { Data = dtoList, Remaining = remaining, Count = total };
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<CountersDto> GetAsync(IOLabAuthorization auth, uint id)
  {
    GetLogger().LogInformation($"ReadAsync id {id}");

    var phys = await GetCounterAsync(id);
    if (phys == null)
      throw new OLabObjectNotFoundException("SystemCounters", id);

    var dto = new CountersFull(GetLogger()).PhysicalToDto(phys);

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskRead, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("CounterMapper", id);

    AttachParentObject(dto);
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
    CountersFullDto dto)
  {
    GetLogger().LogInformation($"PutAsync id {id}");

    dto.ImageableId = dto.ParentInfo.Id;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("CounterMapper", id);

    try
    {
      var builder = new CountersFull(GetLogger());
      var phys = builder.DtoToPhysical(dto);

      phys.UpdatedAt = DateTime.Now;

      GetDbContext().Entry(phys).State = EntityState.Modified;
      await GetDbContext().SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      var existingObject = await GetCounterAsync(id);
      if (existingObject == null)
        throw new OLabObjectNotFoundException("CounterMapper", id);
    }

  }

  /// <summary>
  /// Create new counter
  /// </summary>
  /// <param name="dto">Counter data</param>
  /// <returns>IActionResult</returns>
  public async Task<CountersFullDto> PostAsync(
    IOLabAuthorization auth,
    CountersFullDto dto)
  {
    GetLogger().LogInformation($"PostAsync name = {dto.Name}");

    dto.ImageableId = dto.ParentInfo.Id;
    dto.Value = dto.StartValue;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("CounterMapper", 0);

    var builder = new CountersFull(GetLogger());
    var phys = builder.DtoToPhysical(dto);

    phys.CreatedAt = DateTime.Now;

    GetDbContext().SystemCounters.Add(phys);
    await GetDbContext().SaveChangesAsync();

    dto = builder.PhysicalToDto(phys);

    AttachParentObject(dto);
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
    GetLogger().LogInformation($"DeleteAsync id {id}");

    if (!Exists(id))
      throw new OLabObjectNotFoundException("SystemCounters", id);

    try
    {
      var phys = await GetCounterAsync(id);
      var dto = new CountersFull(GetLogger()).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = await auth.HasAccessAsync(IOLabAuthorization.AclBitMaskWrite, dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("CounterMapper", id);

      GetDbContext().SystemCounters.Remove(phys);
      await GetDbContext().SaveChangesAsync();

    }
    catch (DbUpdateConcurrencyException)
    {
      var existingObject = await GetCounterAsync(id);
      if (existingObject == null)
        throw new OLabObjectNotFoundException("CounterMapper", id);
    }

  }

}
