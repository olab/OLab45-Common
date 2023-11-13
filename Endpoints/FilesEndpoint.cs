using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints
{
  public partial class FilesEndpoint : OLabEndpoint
  {

    public FilesEndpoint(
      IOLabLogger logger,
      IOLabConfiguration configuration,
      OLabDBContext context,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
      IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
        logger,
        configuration,
        context,
        wikiTagProvider,
        fileStorageProvider)
    {
    }

    private bool Exists(uint id)
    {
      return dbContext.SystemFiles.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<FilesDto>> GetAsync(int? take, int? skip)
    {
      Logger.LogDebug($"FilesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var Files = new List<SystemFiles>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      Files = await dbContext.SystemFiles.OrderBy(x => x.Name).ToListAsync();
      total = Files.Count;

      if (take.HasValue && skip.HasValue)
      {
        Files = Files.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      Logger.LogDebug(string.Format("found {0} Files", Files.Count));

      var dtoList = new Files(Logger).PhysicalToDto(Files);

      var maps = GetMapIdNames();
      var nodes = GetNodeIdNames();
      var servers = GetServerIdNames();

      foreach (var dto in dtoList)
        dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

      return new OLabAPIPagedResponse<FilesDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FilesFullDto> GetAsync(
      IOLabAuthorization auth,
      uint id)
    {

      Logger.LogDebug($"FilesController.GetAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Files", id);

      var phys = await dbContext.SystemFiles.FirstAsync(x => x.Id == id);
      var dto = new FilesFull(Logger).PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = auth.HasAccess("R", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Files", id);

      AttachParentObject(dto);
      return dto;

    }

    /// <summary>
    /// Saves a file edit
    /// </summary>
    /// <param name="id">file id</param>
    /// <returns>IActionResult</returns>
    public async Task PutAsync(
      IOLabAuthorization auth,
      uint id, FilesFullDto dto)
    {

      Logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentInfo.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Files", id);

      try
      {
        var builder = new FilesFull(Logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        dbContext.Entry(phys).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Files", id);
      }

    }

    /// <summary>
    /// Create new file
    /// </summary>
    /// <param name="dto">Physical object to save</param>
    /// <returns>FilesFullDto</returns>
    public async Task<FilesFullDto> PostAsync(
      IOLabAuthorization auth,
      FilesFullDto dto,
      CancellationToken cancel)
    {
      Logger.LogDebug($"FilesController.PostAsync()");
      var builder = new FilesFull(Logger);

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Files", 0);

      var phys = builder.DtoToPhysical(dto);
      phys.CreatedAt = DateTime.Now;

      dbContext.SystemFiles.Add(phys);
      await dbContext.SaveChangesAsync();

      using (var stream = new MemoryStream())
      {
        dto.GetFileContents(stream);
        var filePath = $"{dto.ImageableType}{_fileStorageModule.GetFolderSeparator()}{dto.ImageableId}{_fileStorageModule.GetFolderSeparator()}{dto.FileName}";
        await _fileStorageModule.WriteFileAsync(stream, filePath, cancel);
      }

      var newDto = builder.PhysicalToDto(phys);
      return newDto;
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

      Logger.LogDebug($"ConstantsEndpoint.DeleteAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Files", id);

      try
      {
        var phys = await GetFileAsync(id);
        var dto = new FilesFull(Logger).PhysicalToDto(phys);

        // test if user has access to object
        var accessResult = auth.HasAccess("W", dto);
        if (accessResult is UnauthorizedResult)
          throw new OLabUnauthorizedException("Constants", id);

        dbContext.SystemFiles.Remove(phys);
        await dbContext.SaveChangesAsync();

      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetFileAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Files", id);
      }

    }

  }

}

