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
using System.IO;
using Microsoft.Extensions.Options;

namespace OLabWebAPI.Endpoints
{
  public partial class FilesEndpoint : OlabEndpoint
  {
    
    private readonly AppSettings _appSettings;

    public FilesEndpoint( 
      OLabLogger logger, 
      IOptions<AppSettings> appSettings,
      OLabDBContext context, 
      IOlabAuthentication auth) : base(logger, context, auth)
    {
      _appSettings = appSettings.Value;
    }

    private bool Exists(uint id)
    {
      return context.SystemFiles.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetAsync(int? take, int? skip)
    {
      try
      {
        logger.LogDebug($"FilesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

        var Files = new List<SystemFiles>();
        var total = 0;
        var remaining = 0;

        if (!skip.HasValue)
          skip = 0;

        Files = await context.SystemFiles.OrderBy(x => x.Name).ToListAsync();
        total = Files.Count;

        if (take.HasValue && skip.HasValue)
        {
          Files = Files.Skip(skip.Value).Take(take.Value).ToList();
          remaining = total - take.Value - skip.Value;
        }

        logger.LogDebug(string.Format("found {0} Files", Files.Count));

        var dtoList = new ObjectMapper.Files(logger).PhysicalToDto(Files);
        return OLabObjectPagedListResult<FilesDto>.Result(dtoList, remaining);
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
    public async Task<IActionResult> GetAsync(uint id)
    {
      try
      {
        logger.LogDebug($"FilesController.GetAsync(uint id={id})");

        if (!Exists(id))
          return OLabNotFoundResult<uint>.Result(id);

        var phys = await context.SystemFiles.FirstAsync(x => x.Id == id);
        var dto = new ObjectMapper.FilesFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess(dto);
        if (accessResult is UnauthorizedResult)
          return accessResult;

        AttachParentObject(dto);

        return OLabObjectResult<FilesFullDto>.Result(dto);
      }
      catch (Exception ex)
      {
        return OLabServerErrorResult.Result(ex.Message);
      }

    }

    /// <summary>
    /// Saves a file edit
    /// </summary>
    /// <param name="id">file id</param>
    /// <returns>IActionResult</returns>
    public async Task<IActionResult> PutAsync(uint id, FilesFullDto dto)
    {
      try
      {
        logger.LogDebug($"PutAsync(uint id={id})");

        dto.ImageableId = dto.ParentObj.Id;

        // test if user has access to object
        var accessResult = auth.HasAccess(dto);
        if (accessResult is UnauthorizedResult)
          return accessResult;

        var builder = new FilesFull(logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        context.Entry(phys).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return null;

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

      try
      {
        logger.LogDebug($"FilesController.DeleteAsync(uint id={id})");

        if (!Exists(id))
          return OLabNotFoundResult<uint>.Result(id);

        var phys = await GetFileAsync(id);
        var dto = new FilesFull(logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess(dto);
        if (accessResult is UnauthorizedResult)
          return accessResult;

        context.SystemFiles.Remove(phys);
        await context.SaveChangesAsync();

        return null;

      }
      catch (Exception ex)
      {
        return OLabServerErrorResult.Result(ex.Message);
      }

    }

  }

}

