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
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints
{
  public partial class QuestionResponsesEndpoint : OlabEndpoint
  {

    public QuestionResponsesEndpoint(
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
      return dbContext.SystemQuestionResponses.Any(e => e.Id == id);
    }

    /// <summary>
    /// Saves a object edit
    /// </summary>
    /// <param name="id">question id</param>
    /// <returns>IActionResult</returns>
    public async Task PutAsync(
      IOLabAuthentication auth,
      uint id,
      QuestionResponsesDto dto)
    {
      logger.LogDebug($"PutAsync(uint id={id})");

      SystemQuestions physQuestionTemp = await GetQuestionSimpleAsync(dto.QuestionId);
      var builder = new QuestionsFull(logger);
      QuestionsFullDto dtoQuestionTemp = builder.PhysicalToDto(physQuestionTemp);

      // test if user has access to object
      IActionResult accessResult = auth.HasAccess("W", dtoQuestionTemp);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("QuestionResponses", id);

      try
      {
        var responsebuilder = new QuestionResponses(logger, dtoQuestionTemp);
        SystemQuestionResponses physResponse = responsebuilder.DtoToPhysical(dto);

        physResponse.UpdatedAt = DateTime.Now;

        dbContext.SystemQuestionResponses.Update(physResponse);
        await dbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        SystemConstants existingObject = await GetConstantAsync(id);
        if (existingObject == null)
          throw new OLabObjectNotFoundException("QuestionResponses", id);
      }

    }

    /// <summary>
    /// Create new object
    /// </summary>
    /// <param name="dto">object data</param>
    /// <returns>IActionResult</returns>
    public async Task<QuestionResponsesDto> PostAsync(
      IOLabAuthentication auth,
      QuestionResponsesDto dto)
    {
      logger.LogDebug($"QuestionResponsesController.PostAsync({dto.Response})");

      SystemQuestions physQuestionTemp = await GetQuestionSimpleAsync(dto.QuestionId);
      var questionBuilder = new QuestionsFull(logger);
      QuestionsFullDto dtoQuestionTemp = questionBuilder.PhysicalToDto(physQuestionTemp);

      // test if user has access to object
      IActionResult accessResult = auth.HasAccess("W", dtoQuestionTemp);
      if (accessResult is UnauthorizedResult)
        throw new OLabUnauthorizedException("QuestionResponses", 0);

      var responsebuilder = new QuestionResponses(logger, dtoQuestionTemp);
      SystemQuestionResponses physResponse = responsebuilder.DtoToPhysical(dto);

      dbContext.SystemQuestionResponses.Add(physResponse);
      await dbContext.SaveChangesAsync();

      dto = responsebuilder.PhysicalToDto(physResponse);
      return dto;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteAsync(
      IOLabAuthentication auth,
      uint id)
    {
      logger.LogDebug($"QuestionResponsesController.DeleteAsync(uint id={id})");

      if (!Exists(id))
        return OLabNotFoundResult<uint>.Result(id);

      try
      {
        SystemQuestionResponses physResponse = await GetQuestionResponseAsync(id);
        SystemQuestions physQuestion = await GetQuestionAsync(physResponse.QuestionId.Value);
        var questionBuilder = new QuestionsFull(logger);
        QuestionsFullDto dtoQuestion = questionBuilder.PhysicalToDto(physQuestion);

        // test if user has access to objectdtoQuestion
        IActionResult accessResult = auth.HasAccess("W", dtoQuestion);
        if (accessResult is UnauthorizedResult)
          return accessResult;

        dbContext.SystemQuestionResponses.Remove(physResponse);
        await dbContext.SaveChangesAsync();
        return null;
      }
      catch (Exception ex)
      {
        return OLabServerErrorResult.Result(ex.Message);
      }

    }

  }

}
