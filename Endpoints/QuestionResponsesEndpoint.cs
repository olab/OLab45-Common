using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class QuestionResponsesEndpoint : OLabEndpoint
{

  public QuestionResponsesEndpoint(
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
    return dbContext.SystemQuestionResponses.Any(e => e.Id == id);
  }

  /// <summary>
  /// Saves a object edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id,
    QuestionResponsesDto dto)
  {
    Logger.LogInformation($"PutAsync(uint id={id})");

    var physQuestionTemp = await GetQuestionSimpleAsync(dto.QuestionId);
    var builder = new QuestionsFullMapper(Logger);
    var dtoQuestionTemp = builder.PhysicalToDto(physQuestionTemp);

    // test if user has access to object
    var accessResult = auth.HasAccess("W", dtoQuestionTemp);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("QuestionResponses", id);

    try
    {
      var responsebuilder = new QuestionResponses(Logger, dtoQuestionTemp);
      var physResponse = responsebuilder.DtoToPhysical(dto);

      physResponse.UpdatedAt = DateTime.Now;

      dbContext.SystemQuestionResponses.Update(physResponse);
      await dbContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      await GetConstantAsync(id);
    }

  }

  /// <summary>
  /// Create new object
  /// </summary>
  /// <param name="dto">object data</param>
  /// <returns>IActionResult</returns>
  public async Task<QuestionResponsesDto> PostAsync(
    IOLabAuthorization auth,
    QuestionResponsesDto dto)
  {
    Logger.LogInformation($"QuestionResponsesController.PostAsync({dto.Response})");

    var physQuestionTemp = await GetQuestionSimpleAsync(dto.QuestionId);
    var questionBuilder = new QuestionsFullMapper(Logger);
    var dtoQuestionTemp = questionBuilder.PhysicalToDto(physQuestionTemp);

    // test if user has access to object
    var accessResult = auth.HasAccess("W", dtoQuestionTemp);
    if (accessResult is UnauthorizedResult)
      throw new OLabUnauthorizedException("QuestionResponses", 0);

    var responsebuilder = new QuestionResponses(Logger, dtoQuestionTemp);
    var physResponse = responsebuilder.DtoToPhysical(dto);

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
    IOLabAuthorization auth,
    uint id)
  {
    Logger.LogInformation($"QuestionResponsesController.DeleteAsync(uint id={id})");

    if (!Exists(id))
      return OLabNotFoundResult<uint>.Result(id);

    try
    {
      var physResponse = await GetQuestionResponseAsync(id);
      var physQuestion = await GetQuestionAsync(physResponse.QuestionId.Value);
      var questionBuilder = new QuestionsFullMapper(Logger);
      var dtoQuestion = questionBuilder.PhysicalToDto(physQuestion);

      // test if user has access to objectdtoQuestion
      var accessResult = auth.HasAccess("W", dtoQuestion);
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
