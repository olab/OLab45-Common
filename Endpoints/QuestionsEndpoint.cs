using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OLab.Common.Interfaces;

namespace OLab.Api.Endpoints
{
    public partial class QuestionsEndpoint : OLabEndpoint
  {

    public QuestionsEndpoint(
      IOLabLogger logger,
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
      return dbContext.SystemQuestions.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<QuestionsDto>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {

      Logger.LogDebug($"QuestionsController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var physList = new List<SystemQuestions>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      physList = await dbContext.SystemQuestions.OrderBy(x => x.Name).ToListAsync();
      total = physList.Count;

      if (take.HasValue && skip.HasValue)
      {
        physList = physList.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      Logger.LogDebug(string.Format("found {0} questions", physList.Count));

      var dtoList = new Questions(Logger, new WikiTagProvider(Logger)).PhysicalToDto(physList);

      var maps = dbContext.Maps.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
      var nodes = dbContext.MapNodes.Select(x => new IdName() { Id = x.Id, Name = x.Title }).ToList();
      var servers = dbContext.Servers.Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();

      foreach (var dto in dtoList)
        dto.ParentInfo = FindParentInfo(dto.ImageableType, dto.ImageableId, maps, nodes, servers);

      return new OLabAPIPagedResponse<QuestionsDto> { Data = dtoList, Remaining = remaining, Count = total };

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<QuestionsFullDto> GetAsync(
      IOLabAuthentication auth,
      uint id)
    {

      Logger.LogDebug($"QuestionsController.GetAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Questions", id);

      var phys = await dbContext.SystemQuestions.Include("SystemQuestionResponses").FirstAsync(x => x.Id == id);
      var builder = new QuestionsFull(Logger);
      var dto = builder.PhysicalToDto(phys);

      // test if user has access to object
      var accessResult = auth.HasAccess("R", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Questions", id);

      AttachParentObject(dto);

      return dto;

    }

    /// <summary>
    /// Saves a question edit
    /// </summary>
    /// <param name="id">question id</param>
    /// <returns>IActionResult</returns>
    public async Task PutAsync(
      IOLabAuthentication auth,
      uint id,
      QuestionsFullDto dto)
    {
      Logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentInfo.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Questions", id);

      try
      {
        var builder = new QuestionsFull(Logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        dbContext.Entry(phys).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        var existingObject = await GetQuestionAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("Questions", id);
      }

    }

    /// <summary>
    /// Create new question
    /// </summary>
    /// <param name="dto">Question data</param>
    /// <returns>IActionResult</returns>
    public async Task<QuestionsFullDto> PostAsync(
      IOLabAuthentication auth,
      QuestionsFullDto dto)
    {
      Logger.LogDebug($"QuestionsController.PostAsync({dto.Stem})");

      dto.ImageableId = dto.ParentInfo.Id;
      dto.Prompt = !string.IsNullOrEmpty(dto.Prompt) ? dto.Prompt : "";

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Questions", 0);

      var builder = new QuestionsFull(Logger);
      var phys = builder.DtoToPhysical(dto);

      phys.CreatedAt = DateTime.Now;

      dbContext.SystemQuestions.Add(phys);
      await dbContext.SaveChangesAsync();

      dto = builder.PhysicalToDto(phys);
      return dto;

    }

  }

}
