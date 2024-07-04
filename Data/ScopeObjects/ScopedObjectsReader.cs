using Dawn;
using HeyRed.Mime;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Packaging;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data;

public partial class ScopedObjects
{
  /// <summary>
  /// Add a scope level from the database
  /// </summary>
  /// <param name="scopeLevel">Scope level to load</param>
  /// <param name="id">Scope id</param>
  public async Task<ScopedObjects> AddScopeFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    _logger.LogInformation($"  Reading scope {scopeLevel} {id} from database");

    var phys = await LoadAllFromDatabaseAsync(scopeLevel, id);
    Combine(phys);

    return phys;
  }

  /// <summary>
  /// Add scoped objects from a dto
  /// </summary>
  /// <param name="dto">Scoped Objects dto to load</param>
  public void AddScopeFromDto(
    ScopedObjectsDto dto)
  {
    var builder = new ScopedObjectsMapper(GetLogger(), GetDbContext(), GetWikiProvider());

    var phys = builder.DtoToPhysical(dto);
    Combine(phys);
  }

  private async ValueTask<ScopedObjects> LoadAllFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    var phys = new ScopedObjects
    {
      ConstantsPhys = await LoadConstantsFromDatabaseAsync(scopeLevel, id),
      QuestionsPhys = await LoadQuestionsFromDatabaseAsync(scopeLevel, id),
      FilesPhys = await LoadFilesFromDatabaseAsync(scopeLevel, id),
      ScriptsPhys = await LoadScriptsFromDatabaseAsync(scopeLevel, id),
      ThemesPhys = await LoadThemesFromDatabaseAsync(scopeLevel, id),
      CountersPhys = await LoadCountersFromDatabaseAsync(scopeLevel, id, 0),
      CounterActionsPhys = await LoadCounterActionsFromDatabaseAsync(scopeLevel, id)
    };

    return phys;

  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="parentId"></param>
  /// <param name="scopeLevel"></param>
  /// <returns></returns>
  private async Task<List<SystemConstants>> LoadConstantsFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
    var items = new List<SystemConstants>();

    items.AddRange(await _dbContext.SystemConstants.Where(x =>
      x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync());

    if (items.Count > 0)
      _logger.LogInformation($"  constants read {items.Count}");

    return items;
  }

  /// <summary>
  /// Gets question scoped objects
  /// </summary>
  /// <returns></returns>
  private async Task<List<SystemQuestions>> LoadQuestionsFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
    var questionsPhys = new List<SystemQuestions>();

    questionsPhys.AddRange(await _dbContext.SystemQuestions
      .Where(x => x.ImageableType == scopeLevel && x.ImageableId == id)
      .Include("SystemQuestionResponses")
      .ToListAsync());

    // order the responses by Order field
    foreach (var questionPhys in questionsPhys.Where(x => x.SystemQuestionResponses.Count > 0))
    {
      _logger.LogInformation($"  question '{questionPhys.Stem}'. read {questionPhys.SystemQuestionResponses.Count} responses");
      var orderedResponses = questionPhys.SystemQuestionResponses.OrderBy(x => x.Order).ToList();
      questionPhys.SystemQuestionResponses.Clear();
      questionPhys.SystemQuestionResponses.AddRange(orderedResponses);
    }

    return questionsPhys;
  }

  /// <summary>
  /// Gets file scoped objects
  /// </summary>
  /// <param name="fileStorageModule">File storage module that provides URL</param>
  /// <returns></returns>
  private async Task<List<SystemFiles>> LoadFilesFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));

    var items = await _dbContext.SystemFiles.Where(x =>
      x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync();

    // ask the module to add appropriate URLs to files
    if (_fileStorageModule != null)
      _fileStorageModule.AttachUrls(items);

    // enhance the records with mime type
    foreach (var item in items)
    {
      if (string.IsNullOrEmpty(item.Mime))
        item.Mime = MimeTypesMap.GetMimeType(Path.GetFileName(item.Path));
    }

    if (items.Count > 0)
      _logger.LogInformation($"  files read {items.Count}");

    return items;
  }

  /// <summary>
  /// Gets thems scoped objects
  /// </summary>
  /// <returns></returns>
  private async Task<List<SystemThemes>> LoadThemesFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
    var items = new List<SystemThemes>();

    items.AddRange(await _dbContext.SystemThemes.Where(x =>
      x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync());

    if (items.Count > 0)
      _logger.LogInformation($"  themes read {items.Count}");

    return items;
  }

  /// <summary>
  /// Gets script scoped objects
  /// </summary>
  /// <returns>List of SystemScripts</returns>
  private async Task<List<SystemScripts>> LoadScriptsFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
    var items = new List<SystemScripts>();

    items.AddRange(await _dbContext.SystemScripts.Where(x =>
      x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync());

    if (items.Count > 0)
      _logger.LogInformation($"  script read {items.Count}");

    return items;
  }

  /// <summary>
  /// ReadAsync counters associated with a 'parent' object 
  /// </summary>
  /// <param name="sinceTime">(optional) looks for values changed since a (unix) time</param>
  /// <returns>List of counters</returns>
  private async Task<List<SystemCounters>> LoadCountersFromDatabaseAsync(string scopeLevel,
    uint id,
    uint sinceTime = 0)
  {
    Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
    var items = new List<SystemCounters>();

    if (sinceTime != 0)
    {
      // generate DateTime from sinceTime
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      dateTime = dateTime.AddSeconds(sinceTime).ToLocalTime();
      items.AddRange(await _dbContext.SystemCounters.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == id && x.UpdatedAt >= dateTime).ToListAsync());
    }
    else
    {
      items.AddRange(await _dbContext.SystemCounters.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync());
    }

    if (items.Count > 0)
      _logger.LogInformation($"  counters read {items.Count}");

    return items;
  }

  /// <summary>
  /// Gets counter action scoped objects
  /// </summary>
  /// <returns>List of SystemCounterActions</returns>
  private async Task<List<SystemCounterActions>> LoadCounterActionsFromDatabaseAsync(
    string scopeLevel,
    uint id)
  {
    if (ScopeLevel == Api.Utils.Constants.ScopeLevelMap)
    {
      var items = new List<SystemCounterActions>();
      items.AddRange(await _dbContext.SystemCounterActions.Where(x =>
          x.MapId == ScopeId).ToListAsync());

      if (items.Count > 0)
        _logger.LogInformation($"  counter actions read {items.Count}");

      return items;
    }

    return new List<SystemCounterActions>();
  }

}
