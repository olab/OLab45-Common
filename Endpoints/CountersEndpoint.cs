using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Interface;
using OLabWebAPI.Utils;
using OLabWebAPI.Common.Exceptions;

namespace OLabWebAPI.Endpoints
{
  public partial class CountersEndpoint : OlabEndpoint
  {

    public CountersEndpoint(
      OLabLogger logger,
      OLabDBContext context,
      IOlabAuthentication auth) : base(logger, context, auth)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool Exists(uint id)
    {
      return context.SystemCounters.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<CountersDto>> GetAsync(int? take, int? skip)
    {
      logger.LogDebug($"CountersController.GetAsync(int? take={take}, int? skip={skip})");

      var Counters = new List<SystemCounters>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      Counters = await context.SystemCounters.OrderBy(x => x.Name).ToListAsync();
      total = Counters.Count;

      if (take.HasValue && skip.HasValue)
      {
        Counters = Counters.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      logger.LogDebug(string.Format("found {0} Counters", Counters.Count));

      var dtoList = new ObjectMapper.Counters(logger).PhysicalToDto(Counters);
      return new OLabAPIPagedResponse<CountersDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<CountersDto> GetAsync(uint id)
    {
      logger.LogDebug($"CountersController.GetAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Counters", id);

      var phys = await GetCounterAsync(id);
      var dto = new CountersFull(logger).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = auth.HasAccess("R", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Counters", id);

      AttachParentObject(dto);
      return dto;
    }

    /// <summary>
    /// Saves a object edit
    /// </summary>
    /// <param name="id">question id</param>
    /// <returns>IActionResult</returns>
    public async Task PutAsync(uint id, CountersFullDto dto)
    {
      logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Counters", id);

      try
      {
        var builder = new CountersFull(logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        context.Entry(phys).State = EntityState.Modified;
        await context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetCounterAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Counters", id);
      }

    }

    /// <summary>
    /// Create new counter
    /// </summary>
    /// <param name="dto">Counter data</param>
    /// <returns>IActionResult</returns>
    public async Task<CountersFullDto> PostAsync(CountersFullDto dto)
    {
      logger.LogDebug($"CountersController.PostAsync({dto.Name})");

      dto.ImageableId = dto.ParentObj.Id;
      dto.Value = dto.StartValue;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Counters", 0);

      var builder = new CountersFull(logger);
      var phys = builder.DtoToPhysical(dto);

      phys.CreatedAt = DateTime.Now;

      context.SystemCounters.Add(phys);
      await context.SaveChangesAsync();

      dto = builder.PhysicalToDto(phys);

      AttachParentObject(dto);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(uint id)
    {
      logger.LogDebug($"CountersController.DeleteAsync(uint id={id})");

      if (!Exists(id))
          throw new OLabObjectNotFoundException("Counters", id);

      try
      {
        var phys = await GetCounterAsync(id);
        var dto = new CountersFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          throw new OLabUnauthorizedException("Counters", id);

        context.SystemCounters.Remove(phys);
        await context.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetCounterAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Counters", id);
      }

    }

  }

}
