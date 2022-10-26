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
  public partial class QuestionsEndpoint : OlabEndpoint
  {

    public QuestionsEndpoint(
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
      return context.SystemQuestions.Any(e => e.Id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<QuestionsDto>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {

      logger.LogDebug($"QuestionsController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var physList = new List<SystemQuestions>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      physList = await context.SystemQuestions.OrderBy(x => x.Name).ToListAsync();
      total = physList.Count;

      if (take.HasValue && skip.HasValue)
      {
        physList = physList.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      logger.LogDebug(string.Format("found {0} questions", physList.Count));

      var dtoList = new ObjectMapper.Questions(logger, new WikiTagProvider(logger)).PhysicalToDto(physList);
      return new OLabAPIPagedResponse<QuestionsDto> { Data = dtoList, Remaining = remaining, Count = total };

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<QuestionsFullDto> GetAsync(uint id)
    {

      logger.LogDebug($"QuestionsController.GetAsync(uint id={id})");

      if (!Exists(id))
        throw new OLabObjectNotFoundException("Questions", id);

      var phys = await context.SystemQuestions.Include("SystemQuestionResponses").FirstAsync(x => x.Id == id);
      var builder = new QuestionsFull(logger);
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
    public async Task PutAsync(uint id, [FromBody] QuestionsFullDto dto)
    {
      logger.LogDebug($"PutAsync(uint id={id})");

      dto.ImageableId = dto.ParentObj.Id;

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Questions", id);

      try
      {
        var builder = new QuestionsFull(logger);
        var phys = builder.DtoToPhysical(dto);

        phys.UpdatedAt = DateTime.Now;

        context.Entry(phys).State = EntityState.Modified;
        await context.SaveChangesAsync();
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
    public async Task<QuestionsFullDto> PostAsync([FromBody] QuestionsFullDto dto)
    {
      logger.LogDebug($"QuestionsController.PostAsync({dto.Stem})");

      dto.ImageableId = dto.ParentObj.Id;
      dto.Prompt = !string.IsNullOrEmpty(dto.Prompt) ? dto.Prompt : "";

      // test if user has access to object
      var accessResult = auth.HasAccess("W", dto);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("Questions", 0);

      var builder = new QuestionsFull(logger);
      var phys = builder.DtoToPhysical(dto);

      phys.CreatedAt = DateTime.Now;

      context.SystemQuestions.Add(phys);
      await context.SaveChangesAsync();

      dto = builder.PhysicalToDto(phys);
      return dto;

    }

  }

}
