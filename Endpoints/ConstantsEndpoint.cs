using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OLab.Common;
using OLab.Common.Exceptions;
using OLab.Data.Exceptions;
using OLab.Data.Interface;
using OLab.Dto;
using OLab.Model;
using OLab.ObjectMapper;
using OLab.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Endpoints
{
  public partial class ConstantsEndpoint : OlabEndpoint
  {

    public ConstantsEndpoint(
      OLabLogger logger,
      IOptions<AppSettings> appSettings,
      OLabDBContext context) : base(logger, appSettings, context)
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
      IOLabAuthentication auth,
      int? take,
      int? skip)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: ConstantsEndpoint.GetAsync");

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

      logger.LogDebug(string.Format("found {0} Constants", Constants.Count));

      IList<ConstantsDto> dtoList = new ObjectMapper.Constants(logger).PhysicalToDto(Constants);

      var maps = dbContext.Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
      var nodes = dbContext.MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
      var servers = dbContext.Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

      foreach (ConstantsDto dto in dtoList)
        dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

      return new OLabAPIPagedResponse<ConstantsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ConstantsDto> GetAsync(
      IOLabAuthentication auth,
      uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: ConstantsEndpoint.GetAsync");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Constants", id);

      SystemConstants phys = await dbContext.SystemConstants.FirstAsync(x => x.Id == id);
      ConstantsDto dto = new ObjectMapper.Constants(logger).PhysicalToDto(phys);

      // test if user has access to object
      IActionResult accessResult = auth.HasAccess("R", dto);
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
      IOLabAuthentication auth,
      uint id,
      ConstantsDto dto)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: ConstantsEndpoint.PutAsync");

      dto.ImageableId = dto.ParentInfo.Id;

      // test if user has access to object
      IActionResult accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", id);

      try
      {
        var builder = new ConstantsFull(logger);
        SystemConstants phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        dbContext.Entry(phys).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        SystemConstants existingObject = await GetConstantAsync(id);
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
      IOLabAuthentication auth,
      ConstantsDto dto)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: ConstantsEndpoint.PostAsync");

      dto.ImageableId = dto.ParentInfo.Id;

      // test if user has access to object
      IActionResult accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Constants", 0);

      var builder = new ConstantsFull(logger);
      SystemConstants phys = builder.DtoToPhysical(dto);

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
      IOLabAuthentication auth,
      uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: ConstantsEndpoint.DeleteAsync");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Constants", id);

      try
      {
        SystemConstants phys = await GetConstantAsync(id);
        ConstantsDto dto = new ConstantsFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        IActionResult accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          throw new OLabUnauthorizedException("Constants", id);

        dbContext.SystemConstants.Remove(phys);
        await dbContext.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        SystemConstants existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Constants", id);
      }

    }

  }

}
