using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Endpoints;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endpoints.player.ReportEndpoint;

public partial class ReportEndpoint : OLabEndpoint
{
  private IList<UserSessionTraces> _sessionTraces;
  private IList<UserResponses> _sessionResponses;
  private IList<SystemQuestions> _questions;
  private IList<SystemQuestionResponses> _questionsResponses;
  private IList<SystemQuestionTypes> _questionsTypes;
  private IList<MapNodes> _nodes;
  private Maps _map;
  private UserSessions _session;
  private IList<UserState> _userStates;

  public ReportEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context) : base(logger, configuration, context)
  {
  }

  public async Task<SessionReport> GetAsync(
    IOLabAuthorization auth,
    string contextId)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: ReportEndpoint.ReadAsync");

    var dto = new SessionReport();

    _session = await dbContext.UserSessions.FirstOrDefaultAsync(x => x.Uuid == contextId);
    if (_session == null)
      throw new OLabObjectNotFoundException("Session", contextId);

    ReadSessionFromDb(contextId);
    BuildSessionDto(dto);

    return dto;
  }

  private void ReadSessionFromDb(string contextId)
  {
    _sessionTraces = dbContext.UserSessionTraces
      .Where(x => x.SessionId == _session.Id)
      .OrderBy(x => x.DateStamp).ToList();
    Logger.LogInformation($"Found {_sessionTraces.Count} UserSessionTraces records for userSession {contextId}");

    _sessionResponses = dbContext.UserResponses
      .Where(x => x.SessionId == _session.Id)
      .OrderBy(x => x.CreatedAt).ToList();
    Logger.LogInformation($"Found {_sessionResponses.Count} UserResponses records for userSession {contextId}");

    _userStates = dbContext.UserState
      .Where(x => x.SessionId == _session.Id).ToList();
    Logger.LogInformation($"Found {_userStates.Count} UserState records for userSession {contextId}");

    _questionsTypes = dbContext.SystemQuestionTypes.ToList();

    var questionIds = _sessionResponses.Select(x => x.QuestionId).ToList();
    _questions = dbContext.SystemQuestions
      .Where(x => questionIds.Contains(x.Id)).ToList();
    Logger.LogInformation($"Found {_questions.Count} question records for userSession {contextId}");

    _questionsResponses = dbContext.SystemQuestionResponses
      .Where(x => questionIds.Contains(x.QuestionId.Value)).ToList();
    Logger.LogInformation($"Found {_questionsResponses.Count} question correctREsponses records for userSession {contextId}");

    var nodeIds = _sessionTraces.Select(x => x.NodeId).ToList();
    _nodes = dbContext.MapNodes
      .Where(x => nodeIds.Contains(x.Id)).ToList();
    Logger.LogInformation($"Found {_nodes.Count} node records for userSession {contextId}");

    var mapId = _session.MapId;
    _map = dbContext.Maps.FirstOrDefault(x => x.Id == mapId);
  }


  private void BuildSessionDto(SessionReport dto)
  {
    dto.CourseName = _session.CourseName;
    dto.SessionId = _session.Uuid;
    dto.Start = Conversions.GetTime(_session.StartTime);
    if (_session.EndTime.HasValue)
      dto.End = Conversions.GetTime(_session.EndTime.Value);
    dto.UserName = _session.UserId.ToString();

    // build nodes sections
    dto.Nodes = BuildNodesReportDto();
    dto.CheckSum = BuildCheckSum(dto);

  }

  private string BuildCheckSum(SessionReport dto)
  {
    var plainText = "";

    foreach (var counter in dto.Counters)
      plainText += counter.Value;

    foreach (var node in dto.Nodes)
    {
      foreach (var response in node.Responses)
        plainText += response.ResponseText;
    }

    var cypherText = StringUtils.GenerateCheckSum(plainText);
    return cypherText;
  }

  private IList<NodeSession> BuildNodesReportDto()
  {
    var sessionNodes = new List<NodeSession>();

    foreach (var sessionTrace in _sessionTraces)
    {
      var sessionNode = new NodeSession();

      var node = _nodes.FirstOrDefault(x => x.Id == sessionTrace.NodeId);
      sessionNode.NodeName = node.Title;

      sessionNode.NodeId = sessionTrace.NodeId;
      sessionNode.TimeStamp = Conversions.GetTime(sessionTrace.DateStamp.Value);

      // build question correctResponses section
      sessionNode.Responses = BuildNodeResponsesDto(sessionNode.NodeId);

      sessionNodes.Add(sessionNode);
    }

    return sessionNodes;
  }

  private IList<NodeResponse> BuildNodeResponsesDto(uint nodeId)
  {
    var nodeResponses = new List<NodeResponse>();
    var nodeSessionResponses = _sessionResponses.Where(x => x.NodeId == nodeId).ToList();
    var processedConversations = new List<string>();

    foreach (var nodeSessionResponse in nodeSessionResponses)
    {
      var nodeResponse = new NodeResponse();
      nodeResponse.TimeStamp = Conversions.GetTime(nodeSessionResponse.CreatedAt);

      var question = _questions.FirstOrDefault(x => x.Id == nodeSessionResponse.QuestionId);
      var questionResponses =
        _questionsResponses.Where(x => x.QuestionId == question.Id).ToList();

      nodeResponse.QuestionId = question.Id;
      nodeResponse.QuestionName = question.Name;
      var questionType =
        _questionsTypes.FirstOrDefault(x => x.Id == question.EntryTypeId);
      nodeResponse.QuestionType = questionType.Title;
      nodeResponse.QuestionStem = question.Stem;

      // handle special case of DnD or MCQ, which only the last response is
      // displayed so this code jsut removes any previous responses for the question.
      if ((question.EntryTypeId == 6) || (question.EntryTypeId == 3))
      {
        var previousResponse = nodeResponses.FirstOrDefault(x => (x.QuestionId == question.Id));
        if (previousResponse != null)
          nodeResponses.Remove(previousResponse);
      }

      // handle special case of TTalk responses which are handled together
      if (question.EntryTypeId == 15)
      {
        // test if conversation already processed
        if (processedConversations.Contains($"{question.Id}:{nodeId}:{_session.Id}"))
          continue;

        var conversationTextResponses = _sessionResponses
          .Where(x => x.QuestionId == question.Id && x.NodeId == nodeId && x.SessionId == _session.Id).OrderBy(x => x.CreatedAt).ToList();

        foreach (var textResponse in conversationTextResponses)
          nodeResponse.ResponseText += $"{textResponse.Response}<br/>\n";

        processedConversations.Add($"{question.Id}:{nodeId}:{_session.Id}");
      }
      else
      {
        nodeResponse.ResponseText = GetResponseText(
          question,
          questionResponses,
          nodeSessionResponse.Response);
      }

      nodeResponse.CorrectResponse = GetQuestionCorrectResponse(
        question,
        questionResponses.Where(x => x.QuestionId == question.Id).ToList());

      nodeResponses.Add(nodeResponse);
    }

    return nodeResponses;
  }

  private string GetResponseText(
    SystemQuestions question,
    IList<SystemQuestionResponses> questionResponses,
    string response)
  {
    var responseText = "???";

    switch (question.EntryTypeId)
    {
      case 1:
      case 2:
      case 10:
        responseText = response;
        break;
      case 3:
        responseText = ProcessMultipleChoiceQuestion(question, questionResponses, response);
        break;
      case 4:
        responseText = ProcessPickChoiceQuestion(question, questionResponses, response);
        break;
      case 5:
        responseText = ProcessSliderQuestion(question, questionResponses, response);
        break;
      case 6:
        responseText = ProcessDragAndDropQuestion(question, questionResponses, response);
        break;
      case 12:
        responseText = ProcessDropDownQuestion(question, questionResponses, response);
        break;
      case 15:
        responseText = ProcessTurkTalkQuestion(question, questionResponses, response);
        break;
    }

    return responseText;
  }

  private string ProcessSliderQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    return response;
  }

  private string ProcessTurkTalkQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    return response;
  }

  private string ProcessDropDownQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    return ProcessPickChoiceQuestion(question, questionResponses, response);
  }

  private string ProcessDragAndDropQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    var responseIdStrings = response.Split(',');
    var responseTexts = new List<string>();

    foreach (var responseIdString in responseIdStrings)
    {
      var responseId = 0;
      if (!Int32.TryParse(responseIdString, out responseId))
        responseTexts.Add(responseIdString.ToString());
      else
      {
        var questionResponse = _questionsResponses.FirstOrDefault(x => x.Id == responseId);
        responseTexts.Add(questionResponse.Response);
      }
    }

    return string.Join("<br/><br/>", responseTexts);
  }

  private string ProcessPickChoiceQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    var responseId = 0;
    if (!Int32.TryParse(response, out responseId))
      return response;

    var questionResponse = _questionsResponses.FirstOrDefault(x => x.Id == responseId);
    return questionResponse.Response;
  }

  private string ProcessMultipleChoiceQuestion(SystemQuestions question, IList<SystemQuestionResponses> questionResponses, string response)
  {
    var responseIdStrings = response.Split(",");
    var responsesText = new List<string>();

    foreach (var responseIdString in responseIdStrings)
    {
      var responseId = 0;
      // if any non-id responses, then this probably isn't a MCQ
      if (!Int32.TryParse(responseIdString, out responseId))
        return response;

      var questionResponse = _questionsResponses.FirstOrDefault(x => x.Id == responseId);
      responsesText.Add(questionResponse.Response);

    }

    return string.Join("<br/>", responsesText);

  }

  private string GetQuestionCorrectResponse(
    SystemQuestions question,
    IList<SystemQuestionResponses> questionResponses)
  {
    var correctResponses = questionResponses.Where(x => x.IsCorrect.HasValue && (x.IsCorrect.Value == 1)).ToList();

    if (correctResponses.Count == 0)
      return "N/A";

    var responseTexts = correctResponses.Select(x => x.Response).ToList();
    var correctText = string.Join(',', responseTexts);
    return correctText;
  }

}
