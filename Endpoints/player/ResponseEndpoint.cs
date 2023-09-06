using Microsoft.Extensions.Options;
using OLab.Api.Common.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using System;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class ResponseEndpoint : OlabEndpoint
  {

    public ResponseEndpoint(
      OLabLogger logger,
      IOptions<AppSettings> appSettings, OLabDBContext context) : base(logger, appSettings, context)
    {
    }

    /// <summary>
    /// Save a question response to the session
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<DynamicScopedObjectsDto> PostQuestionResponseAsync(
      SystemQuestions question,
      QuestionResponsePostDataDto body)
    {
      logger.LogDebug($"PostQuestionResponseAsync(questionId={body.QuestionId}, response={body.PreviousValue}->{body.Value}");

      // dump out original dynamic objects for logging
      body.DynamicObjects.Dump(logger, "Response Original");

      // test if counter associated with the question
      if (question.CounterId.HasValue && (question.CounterId.Value > 0))
      {
        SystemCounters dbCounter = await GetCounterAsync(question.CounterId.Value);
        if (dbCounter == null)
          throw new Exception($"Counter {question.CounterId.Value} not found");

        CountersDto counterDto = GetTargetCounter(question, dbCounter, body);

        if (question.SystemQuestionResponses.Count > 0)
        {
          if (question.EntryTypeId == 4)
            // handle questions that have a single response
            ProcessSingleResponseQuestion(question, counterDto, body);

          else if (question.EntryTypeId == 3)
            // handle questions that have multiple responses
            ProcessMultipleResponseQuestion(question, counterDto, body);

          else if (question.EntryTypeId == 12)
            // handle questions that have a slider
            ProcessSingleResponseQuestion(question, counterDto, body);

          else
            throw new OLabGeneralException($"question {question.Id} not implemented");
        }
        else
          // handle questions that have no underlying responses (e.g. slider)
          ProcessValueQuestion(question, counterDto, body);

        // if a server-level counter value has changed, write it to db
        if (dbCounter.ImageableType == Utils.Constants.ScopeLevelServer)
        {
          dbCounter.ValueFromNumber(counterDto.ValueAsNumber());
          dbContext.SystemCounters.Update(dbCounter);
          await dbContext.SaveChangesAsync();
        }
      }
      else
        logger.LogWarning($"question {question.Id} response: question has no counter");

      // update dynamic object checksum since counter values
      // map have changed
      body.DynamicObjects.RefreshChecksum();

      // dump out original dynamic objects for logging
      body.DynamicObjects.Dump(logger, "Response New");

      return body.DynamicObjects;

    }

    /// <summary>
    /// Get counter for question
    /// </summary>
    /// <param name="question">Source question</param>
    /// <param name="dbCounter">Database Counter</param>
    /// <param name="body">Request body</param>
    /// <returns>Dto counter</returns>
    private CountersDto GetTargetCounter(SystemQuestions question, SystemCounters dbCounter, QuestionResponsePostDataDto body)
    {
      CountersDto dynamicCounter = body.DynamicObjects.GetCounter(question.CounterId.Value);
      if (dynamicCounter == null)
        logger.LogError($"Counter {question.CounterId.Value} not found in request. Update ignored");
      else
      {
        // if counter is server-level, then take db value and copy
        // it to dynamic object version, which is passed back to caller
        if (dbCounter.ImageableType == Utils.Constants.ScopeLevelServer)
          dynamicCounter.Value = dbCounter.ValueAsString();
      }

      return dynamicCounter;
    }

    /// <summary>
    /// Process multiple response-based question
    /// </summary>
    /// <param name="question">Source question</param>
    /// <param name="counterDto">Counter Dto</param>
    /// <param name="body">Request body</param>
    private void ProcessMultipleResponseQuestion(SystemQuestions question, CountersDto counterDto, QuestionResponsePostDataDto body)
    {
      // test for no active counter
      if (counterDto == null)
        return;

      if (string.IsNullOrEmpty(body.Value))
        return;

      var score = question.GetScoreFromResponses(body.Value);

      logger.LogDebug($"counter {counterDto.Id} value = {score}");
      counterDto.SetValue(score);
    }

    /// <summary>
    /// Process value-based question
    /// </summary>
    /// <param name="question">Source question</param>
    /// <param name="counterDto">Counter Dto</param>
    /// <param name="body">Request body</param>
    private void ProcessValueQuestion(SystemQuestions question, CountersDto counterDto, QuestionResponsePostDataDto body)
    {
      // test for no active counter
      if (counterDto == null)
        return;
    }

    /// <summary>
    /// Process single response-based question
    /// </summary>
    /// <param name="question">Source question</param>
    /// <param name="counterDto">Counter Dto</param>
    /// <param name="body">Request body</param>
    private void ProcessSingleResponseQuestion(SystemQuestions question, CountersDto counterDto, QuestionResponsePostDataDto body)
    {
      // test for no active counter
      if (counterDto == null)
        return;

      SystemQuestionResponses currentResponse = null;
      SystemQuestionResponses previousResponse = null;
      var currentScore = 0;
      var previousScore = 0;

      var score = counterDto.ValueAsNumber();

      if (body.ResponseId.HasValue)
      {
        currentResponse = question.GetResponse(body.ResponseId.Value);
        if (currentResponse == null)
          throw new Exception($"Question response {body.ResponseId.Value} not found");

        if (currentResponse.Score.HasValue)
          currentScore = currentResponse.Score.Value;
        else
          logger.LogWarning($"Response {body.ResponseId.Value} does not have a score value");
      }
      else
        throw new Exception($"Question id = {question.Id} current response not valid.");

      if (body.PreviousResponseId.HasValue && (body.PreviousResponseId.Value > 0))
      {
        previousResponse = question.GetResponse(body.PreviousResponseId.Value);
        if (previousResponse == null)
          throw new Exception($"Question previous response {body.PreviousResponseId.Value} not found");

        if (previousResponse.Score.HasValue)
          previousScore = previousResponse.Score.Value;
        else
          logger.LogWarning($"Response {body.ResponseId.Value} does not have a score value");
      }

      // back out any previous response value
      if (previousResponse != null)
      {
        logger.LogDebug($"reverting previous question reponse {body.PreviousResponseId.Value} = {previousScore}");
        score -= previousScore;
      }

      // add in current response score
      if (currentResponse != null)
      {
        logger.LogDebug($"adjusting question with reponse {body.ResponseId.Value} = {currentScore}");
        score += currentScore;
      }

      logger.LogDebug($"counter {counterDto.Id} value = {score}");

      counterDto.SetValue(score);

    }

  }
}

