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
  public partial class ConstantsEndpoint : OlabEndpoint
  {

    public ConstantsEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool Exists(uint id)
    {
      return context.SystemConstants.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<ConstantsDto>> GetAsync(int? take, int? skip)
    {
      logger.LogDebug($"ConstantsEndpoint.GetAsync(int? take={take}, int? skip={skip})");

      var Constants = new List<SystemConstants>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      Constants = await context.SystemConstants.OrderBy(x => x.Name).ToListAsync();
      total = Constants.Count;

      if (take.HasValue && skip.HasValue)
      {
        Constants = Constants.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      logger.LogDebug(string.Format("found {0} Constants", Constants.Count));

      var dtoList = new ObjectMapper.Constants(logger).PhysicalToDto(Constants);
      return new OLabAPIPagedResponse<ConstantsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ConstantsDto> GetAsync(
      IOlabAuthentication auth, 
      uint id)
    {
      logger.LogDebug($"ConstantsEndpoint.GetAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Constants", id);

      var phys = await context.SystemConstants.FirstAsync(x => x.Id == id);
      var dto = new ObjectMapper.Constants(logger).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = auth.HasAccess("R", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", id);

      AttachParentObject(dto);

      return dto;
    }

    /// <summary>
    /// Saves a object edit
    /// </summary>
    /// <param name="id">question id</param>
    /// <returns>IActionResult</returns>
    public async Task PutAsync(
      IOlabAuthentication auth, 
      uint id, 
      ConstantsDto dto)
    {
      logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", id);

      try
      {
        var builder = new ConstantsFull(logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        context.Entry(phys).State = EntityState.Modified;
        await context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Constants", id);
      }

    }

    /// <summary>
    /// Create new object
    /// </summary>
    /// <param name="dto">object data</param>
    /// <returns>IActionResult</returns>
    public async Task<ConstantsDto> PostAsync(
      IOlabAuthentication auth, 
      ConstantsDto dto)
    {
      logger.LogDebug($"ConstantsEndpoint.PostAsync({dto.Name})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", 0);

      var builder = new ConstantsFull(logger);
      var phys = builder.DtoToPhysical(dto);

      phys.CreatedAt = DateTime.Now;

      context.SystemConstants.Add(phys);
      await context.SaveChangesAsync();

      dto = builder.PhysicalToDto(phys);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(
      IOlabAuthentication auth, 
      uint id)
    {
      logger.LogDebug($"ConstantsEndpoint.DeleteAsync(uint id={id})");

      if (!Exists(id))
          throw new OLabObjectNotFoundException("Constants", id);

      try
      {
        var phys = await GetConstantAsync(id);
        var dto = new ConstantsFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          throw new OLabUnauthorizedException("Constants", id);

        context.SystemConstants.Remove(phys);
        await context.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Constants", id);
      }

    }

  }

}
