using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
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
    OLabDBContext context) : base( logger, configuration, context )
  {
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  private bool Exists(uint id)
  {
    return GetDbContext().SystemQuestionResponses.Any( e => e.Id == id );
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
    GetLogger().LogInformation( $"PutAsync(uint id={id})" );

    var physQuestionTemp = await GetQuestionSimpleAsync( dto.QuestionId );
    var builder = new QuestionsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
    var dtoQuestionTemp = builder.PhysicalToDto( physQuestionTemp );

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dtoQuestionTemp );
    if ( accessResult is UnauthorizedResult )
      throw new OLabUnauthorizedException( "QuestionResponses", id );

    try
    {
      var responsebuilder = new QuestionResponses(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(), dtoQuestionTemp );
      var physResponse = responsebuilder.DtoToPhysical( dto );

      physResponse.UpdatedAt = DateTime.Now;

      GetDbContext().SystemQuestionResponses.Update( physResponse );
      await GetDbContext().SaveChangesAsync();
    }
    catch ( DbUpdateConcurrencyException )
    {
      await GetConstantAsync( id );
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
    GetLogger().LogInformation( $"QuestionResponsesController.PostAsync({dto.Response})" );

    var physQuestionTemp = await GetQuestionSimpleAsync( dto.QuestionId );
    var questionBuilder = new QuestionsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
    var dtoQuestionTemp = questionBuilder.PhysicalToDto( physQuestionTemp );

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dtoQuestionTemp );
    if ( accessResult is UnauthorizedResult )
      throw new OLabUnauthorizedException( "QuestionResponses", 0 );

    var responsebuilder = new QuestionResponses(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(), dtoQuestionTemp );
    var physResponse = responsebuilder.DtoToPhysical( dto );

    GetDbContext().SystemQuestionResponses.Add( physResponse );
    await GetDbContext().SaveChangesAsync();

    dto = responsebuilder.PhysicalToDto( physResponse );
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
    GetLogger().LogInformation( $"QuestionResponsesController.DeleteAsync(uint id={id})" );

    if ( !Exists( id ) )
      return OLabNotFoundResult<uint>.Result( id );

    try
    {
      var physResponse = await GetQuestionResponseAsync( id );
      var physQuestion = await GetQuestionAsync( physResponse.QuestionId.Value );
      var questionBuilder = new QuestionsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
      var dtoQuestion = questionBuilder.PhysicalToDto( physQuestion );

      // test if user has access to objectdtoQuestion
      var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dtoQuestion );
      if ( accessResult is UnauthorizedResult )
        return accessResult;

      GetDbContext().SystemQuestionResponses.Remove( physResponse );
      await GetDbContext().SaveChangesAsync();
      return null;
    }
    catch ( Exception ex )
    {
      return OLabServerErrorResult.Result( ex.Message );
    }

  }

}
