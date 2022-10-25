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

namespace OLabWebAPI.Endpoints
{
  public partial class ConstantsEndpoint : OlabEndpoint
  {

    public ConstantsEndpoint( 
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
      return context.SystemConstants.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetAsync(int? take, int? skip)
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
      return OLabObjectPagedListResult<ConstantsDto>.Result(dtoList, remaining);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetAsync(uint id)
    {
      logger.LogDebug($"ConstantsEndpoint.GetAsync(uint id={id})");

      if (!Exists(id))
        return OLabNotFoundResult<uint>.Result(id);

      var phys = await context.SystemConstants.FirstAsync(x => x.Id == id);
      var dto = new ObjectMapper.Constants(logger).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = auth.HasAccess("R", dto);
      if (accessResult is UnauthorizedResult)
        return accessResult;

      AttachParentObject(dto);

      return OLabObjectResult<ConstantsDto>.Result(dto);
    }

    /// <summary>
    /// Saves a object edit
    /// </summary>
    /// <param name="id">question id</param>
    /// <returns>IActionResult</returns>
    public async Task<IActionResult> PutAsync(uint id, ConstantsDto dto)
    {
      logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        return accessResult;

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
          return OLabNotFoundResult<uint>.Result(id);
      }

      return null;

    }

    /// <summary>
    /// Create new object
    /// </summary>
    /// <param name="dto">object data</param>
    /// <returns>IActionResult</returns>
    public async Task<IActionResult> PostAsync(ConstantsDto dto)
    {
      logger.LogDebug($"ConstantsEndpoint.PostAsync({dto.Name})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        return accessResult;

      try
      {
        var builder = new ConstantsFull(logger);
        var phys = builder.DtoToPhysical(dto);

        phys.CreatedAt = DateTime.Now;

        context.SystemConstants.Add(phys);
        await context.SaveChangesAsync();

        dto = builder.PhysicalToDto(phys);
        return OLabObjectResult<ConstantsDto>.Result(dto);

      }
      catch (Exception ex)
      {
        return OLabServerErrorResult.Result(ex.Message);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteAsync(uint id)
    {
      logger.LogDebug($"ConstantsEndpoint.DeleteAsync(uint id={id})");

      if (!Exists(id))
        return OLabNotFoundResult<uint>.Result(id);

      try
      {
        var phys = await GetConstantAsync(id);
        var dto = new ConstantsFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          return accessResult;

        context.SystemConstants.Remove(phys);
        await context.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          return OLabNotFoundResult<uint>.Result(id);
      }

      return null;
    }

  }

}
