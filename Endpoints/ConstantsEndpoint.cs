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

namespace OLab.Api.Endpoints
{
  public partial class ConstantsEndpoint : OLabEndpoint
  {

    public ConstantsEndpoint(
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
      return dbContext.SystemConstants.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<ConstantsDto>> GetAsync(
      IOLabAuthorization auth,
      int? take,
      int? skip)
    {
      Logger.LogDebug($"{auth.UserContext.UserId}: ConstantsEndpoint.ReadAsync");

      var Constants = new List<SystemConstants>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      Constants = await dbContext.SystemConstants.OrderBy(x => x.Name).ToListAsync();
      total = Constants.Count;

      if (take.HasValue && skip.HasValue)
      {
        Constants = Constants.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      Logger.LogDebug(string.Format("found {0} Constants", Constants.Count));

      var dtoList = new ObjectMapper.Constants(Logger).PhysicalToDto(Constants);

      var maps = dbContext.Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
      var nodes = dbContext.MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
      var servers = dbContext.Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

      foreach (var dto in dtoList)
        dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

      return new OLabAPIPagedResponse<ConstantsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ConstantsDto> GetAsync(
      IOLabAuthorization auth,
      uint id)
    {
      Logger.LogDebug($"{auth.UserContext.UserId}: ConstantsEndpoint.ReadAsync");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Constants", id);

      var phys = await dbContext.SystemConstants.FirstAsync(x => x.Id == id);
      var dto = new ObjectMapper.Constants(Logger).PhysicalToDto(phys);

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
      IOLabAuthorization auth,
      uint id,
      ConstantsDto dto)
    {
      Logger.LogDebug($"{auth.UserContext.UserId}: ConstantsEndpoint.PutAsync");

      dto.ImageableId = dto.ParentInfo.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", id);

      try
      {
        var builder = new ConstantsFull(Logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        dbContext.Entry(phys).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id)
          ?? throw new OLabObjectNotFoundException("Constants", id);
      }

    }

    /// <summary>
    /// Create new object
    /// </summary>
    /// <param name="dto">object data</param>
    /// <returns>IActionResult</returns>
    public async Task<ConstantsDto> PostAsync(
      IOLabAuthorization auth,
      ConstantsDto dto)
    {
      Logger.LogDebug($"{auth.UserContext.UserId}: ConstantsEndpoint.PostAsync");

      dto.ImageableId = dto.ParentInfo.Id != 0 ? dto.ParentInfo.Id : dto.ImageableId;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", 0);

      var builder = new ConstantsFull(Logger);
      var phys = builder.DtoToPhysical(dto);

      phys.CreatedAt = DateTime.Now;

      dbContext.SystemConstants.Add(phys);
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
      Logger.LogDebug($"{auth.UserContext.UserId}: ConstantsEndpoint.DeleteAsync");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Constants", id);

      try
      {
        var phys = await GetConstantAsync(id);
        var dto = new ConstantsFull(Logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          throw new OLabUnauthorizedException("Constants", id);

        dbContext.SystemConstants.Remove(phys);
        await dbContext.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id)
          ?? throw new OLabObjectNotFoundException("Constants", id);
      }

    }

  }

}
