using Dawn;
using DocumentFormat.OpenXml.Spreadsheet;
using HeyRed.Mime;
using Humanizer;
using Microsoft.EntityFrameworkCore;
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

namespace OLab.Data.BusinessObjects
{
  public partial class ScopedObjects
  {
    public IOLabLogger Logger { get; }
    private readonly IDictionary<uint, SystemCounters> _nodeIdTranslation =
      new Dictionary<uint, SystemCounters>();
    private OLabDBContext _dbContext { get; set; }

    public uint parentId { get; }
    public string scopeLevel { get; }

    public List<SystemConstants> Constants { get; set; }
    public List<SystemCounters> Counters { get; set; }
    public List<SystemCounterActions> CounterActions { get; set; }
    public List<SystemFiles> Files { get; set; }
    public List<SystemQuestions> Questions { get; set; }
    public List<SystemScripts> Scripts { get; set; }
    public List<SystemThemes> Themes { get; set; }

    private readonly IDictionary<string, ScopedObjects> scopedObjectList = new Dictionary<string, ScopedObjects>();

    /// <summary>
    /// Retrieves and combines multiple levels of scoped objects
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="serverId"></param>
    /// <param name="mapId"></param>
    /// <param name="nodeId"></param>
    public ScopedObjects(
      IOLabLogger logger,
      OLabDBContext dbContext,
      uint? serverId,
      uint? mapId,
      uint? nodeId) : this()
    {
      Logger = logger;
      this._dbContext = dbContext;

      if (serverId.HasValue)
        scopedObjectList.Add(
            Api.Utils.Constants.ScopeLevelServer,
            new ScopedObjects(Logger, dbContext, serverId.Value, Api.Utils.Constants.ScopeLevelServer));

      if (mapId.HasValue)
        scopedObjectList.Add(
            Api.Utils.Constants.ScopeLevelMap,
            new ScopedObjects(Logger, dbContext, mapId.Value, Api.Utils.Constants.ScopeLevelMap));

      if (nodeId.HasValue)
        scopedObjectList.Add(
            Api.Utils.Constants.ScopeLevelNode,
            new ScopedObjects(Logger, dbContext, nodeId.Value, Api.Utils.Constants.ScopeLevelNode));
    }

    /// <summary>
    /// Builds a single level of scoped objects
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="parentId"></param>
    /// <param name="scopeLevel"></param>
    public ScopedObjects(
      IOLabLogger logger,
      OLabDBContext dbContext,
      uint parentId,
      string scopeLevel) : this()
    {
      Logger = logger;
      this._dbContext = dbContext;
      this.parentId = parentId;
      this.scopeLevel = scopeLevel;

      scopedObjectList.Add(
          scopeLevel,
          this);
    }

    /// <summary>
    /// Default ctor needed for Mapping
    /// </summary>
    public ScopedObjects()
    {
      Constants = new List<SystemConstants>();
      Counters = new List<SystemCounters>();
      CounterActions = new List<SystemCounterActions>();
      Questions = new List<SystemQuestions>();
      Files = new List<SystemFiles>();
      Scripts = new List<SystemScripts>();
      Themes = new List<SystemThemes>();

      // MapAvatars = new HashSet<MapAvatars>();
      // MapChats = new HashSet<MapChats>();
      // MapCollectionMaps = new HashSet<MapCollectionMaps>();
      // MapContributors = new HashSet<MapContributors>();
      // MapCounterCommonRules = new HashSet<MapCounterCommonRules>();
      // MapDams = new HashSet<MapDams>();
      // MapElements = new HashSet<MapElements>();
      // MapFeedbackRules = new HashSet<MapFeedbackRules>();
      // MapKeys = new HashSet<MapKeys>();
      // MapNodeJumps = new HashSet<MapNodeJumps>();
      // MapNodeLinks = new HashSet<MapNodeLinks>();
      // MapNodeSections = new HashSet<MapNodeSections>();
      // MapUsers = new HashSet<MapUsers>();
      // QCumulative = new HashSet<QCumulative>();
      // ScenarioMaps = new HashSet<ScenarioMaps>();
      // UserSessions = new HashSet<UserSessions>();
      // UserSessionTraces = new HashSet<UserSessionTraces>();
      // UserState = new HashSet<UserState>();
    }

    /// <summary>
    /// Appends a ScopedObjectsMapper to the current one
    /// </summary>
    /// <param name="source">Source ScopedObjectsMapper</param>
    public void Combine(ScopedObjects source)
    {
      Constants.AddRange(source.Constants);
      Counters.AddRange(source.Counters);
      CounterActions.AddRange(source.CounterActions);
      Questions.AddRange(source.Questions);
      Files.AddRange(source.Files);
      Scripts.AddRange(source.Scripts);
      Themes.AddRange(source.Themes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="scopeLevel"></param>
    /// <returns></returns>
    private async Task<List<SystemConstants>> ReadConstantsAsync()
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
      var items = new List<SystemConstants>();

      items.AddRange(await _dbContext.SystemConstants.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      if (items.Count > 0)
        Logger.LogInformation($"  constants read {items.Count}");

      return items;
    }

    /// <summary>
    /// Gets question scoped objects
    /// </summary>
    /// <returns></returns>
    private async Task<List<SystemQuestions>> ReadQuestionsAsync()
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
      var items = new List<SystemQuestions>();

      items.AddRange(await _dbContext.SystemQuestions
        .Where(x => x.ImageableType == scopeLevel && x.ImageableId == parentId)
        .Include("SystemQuestionResponses")
        .ToListAsync());

      // order the responses by Order field
      foreach (var item in items)
      {
        Logger.LogInformation($"  question '{item.Stem}'. read {item.SystemQuestionResponses.Count} responses");
        item.SystemQuestionResponses = item.SystemQuestionResponses.OrderBy(x => x.Order).ToList();
      }

      return items;
    }

    /// <summary>
    /// Gets file scoped objects
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns></returns>
    private async Task<List<SystemFiles>> ReadFileAsync(
      IFileStorageModule fileStorageModule = null)
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));

      var items = await _dbContext.SystemFiles.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync();

      // ask the module to add appropriate URLs to files
      if (fileStorageModule != null)
        fileStorageModule.AttachUrls(items);

      // enhance the records with mime type
      foreach (var item in items)
      {
        if (string.IsNullOrEmpty(item.Mime))
          item.Mime = MimeTypesMap.GetMimeType(Path.GetFileName(item.Path));
      }

      if (items.Count > 0)
        Logger.LogInformation($"  files read {items.Count}");

      return items;
    }

    /// <summary>
    /// Gets thems scoped objects
    /// </summary>
    /// <returns></returns>
    private async Task<List<SystemThemes>> ReadThemesAsync()
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
      var items = new List<SystemThemes>();

      items.AddRange(await _dbContext.SystemThemes.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      if (items.Count > 0)
        Logger.LogInformation($"  themes read {items.Count}");

      return items;
    }

    /// <summary>
    /// Gets script scoped objects
    /// </summary>
    /// <returns>List of SystemScripts</returns>
    private async Task<List<SystemScripts>> ReadScriptsAsync()
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
      var items = new List<SystemScripts>();

      items.AddRange(await _dbContext.SystemScripts.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      if (items.Count > 0)
        Logger.LogInformation($"  script read {items.Count}");

      return items;
    }

    /// <summary>
    /// ReadAsync counters associated with a 'parent' object 
    /// </summary>
    /// <param name="sinceTime">(optional) looks for values changed since a (unix) time</param>
    /// <returns>List of counters</returns>
    private async Task<List<SystemCounters>> ReadCountersAsync(uint sinceTime = 0)
    {
      Guard.Argument(_dbContext).NotNull(nameof(_dbContext));
      var items = new List<SystemCounters>();

      if (sinceTime != 0)
      {
        // generate DateTime from sinceTime
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(sinceTime).ToLocalTime();
        items.AddRange(await _dbContext.SystemCounters.Where(x =>
          x.ImageableType == scopeLevel && x.ImageableId == parentId && x.UpdatedAt >= dateTime).ToListAsync());
      }
      else
      {
        items.AddRange(await _dbContext.SystemCounters.Where(x =>
          x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());
      }

      if (items.Count > 0)
        Logger.LogInformation($"  counters read {items.Count}");

      return items;
    }

    /// <summary>
    /// Gets counter action scoped objects
    /// </summary>
    /// <returns>List of SystemCounterActions</returns>
    private async Task<List<SystemCounterActions>> ReadCounterActionsAsync()
    {
      if (scopeLevel == Api.Utils.Constants.ScopeLevelMap)
      {
        var items = new List<SystemCounterActions>();
        items.AddRange(await _dbContext.SystemCounterActions.Where(x =>
            x.MapId == parentId).ToListAsync());

        if (items.Count > 0)
          Logger.LogInformation($"  counter actions read {items.Count}");

        return items;
      }

      return new List<SystemCounterActions>();
    }

    private async ValueTask<ScopedObjects> ReadInternalAsync(
      IFileStorageModule fileStorageModule = null)
    {
      var phys = new ScopedObjects
      {
        Constants = await ReadConstantsAsync(),
        Questions = await ReadQuestionsAsync(),
        Files = await ReadFileAsync(fileStorageModule),
        Scripts = await ReadScriptsAsync(),
        Themes = await ReadThemesAsync(),
        Counters = await ReadCountersAsync(0),
        CounterActions = await ReadCounterActionsAsync()
      };

      return phys;

    }

    /// <summary>
    /// ReadAsync scoped objects for a specific scope level
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns>ScopedObject dto</returns>
    public async ValueTask<ScopedObjects> ReadAsync(
      string scopeLevel,
      IFileStorageModule fileStorageModule = null)
    {
      return await scopedObjectList[scopeLevel].ReadInternalAsync(fileStorageModule);
    }

    /// <summary>
    /// ReadAsync scoped objects for all levels
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns>ScopedObject dto</returns>
    public async ValueTask<ScopedObjects> ReadAsync(
      IFileStorageModule fileStorageModule = null)
    {
      var phys = new ScopedObjects();
      foreach (var item in scopedObjectList)
      {
        var typePhys = await ReadAsync(item.Key, fileStorageModule);
        phys.Combine(typePhys);
      }

      return phys;
    }

    /// <summary>
    /// Writes a dto to the database
    /// </summary>
    /// <param name="dto">Scoped Objects dto</param>
    /// <param name="token">Cancellation token</param>
    public async Task WriteAsync(ScopedObjectsDto dto, CancellationToken token)
    {
      foreach (var questionDto in dto.Questions)
        await WriteQuestionsAsync(
          questionDto,
          token);

      foreach (var constantDto in dto.Constants)
        await WriteConstantAsync(
          constantDto,
          token);

      foreach (var fileDto in dto.Files)
        await WriteFileAsync(
          fileDto,
          token);

      _nodeIdTranslation.Clear();
      foreach (var counterDto in dto.Counters)
        await WriteCounterAsync(
          counterDto,
          token);

      foreach (var actionDto in dto.CounterActions)
        await WriteActionAsync(
          actionDto,
          token);
    }

    private async Task WriteActionAsync(CounterActionsDto dto, CancellationToken token)
    {
      var phys = new CounterActionsMapper(Logger).DtoToPhysical(dto);

      phys.Id = 0;
      phys.CounterId = _nodeIdTranslation[dto.Id].Id;
      phys.ImageableId = _nodeIdTranslation[dto.Id].ImageableId;
      phys.ImageableType = _nodeIdTranslation[dto.Id].ImageableType;

      await _dbContext.SystemCounterActions.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);
    }

    private async Task WriteCounterAsync(CountersDto dto, CancellationToken token)
    {
      var phys = new Counters(Logger).DtoToPhysical(dto);

      phys.Id = 0;
      phys.ImageableType = scopeLevel;
      phys.ImageableId = parentId;

      await _dbContext.SystemCounters.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      // save the new counter id since creating
      // counter actions will need this mapping.
      _nodeIdTranslation.Add(dto.Id.Value, phys);

      Logger.LogInformation($"  imported counter '{phys.Name}'");
    }

    private async Task WriteFileAsync(
      FilesFullDto dto,
      CancellationToken token)
    {
      var phys = new FilesFull(Logger).DtoToPhysical(dto);

      phys.Id = 0;
      phys.ImageableType = scopeLevel;
      phys.ImageableId = parentId;

      await _dbContext.SystemFiles.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported file '{phys.Name}'");
    }

    private async Task WriteConstantAsync(
      ConstantsDto dto,
      CancellationToken token)
    {
      var phys = new ConstantsFull(Logger).DtoToPhysical(dto);

      phys.Id = 0;
      phys.ImageableType = scopeLevel;
      phys.ImageableId = parentId;

      await _dbContext.SystemConstants.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported constant {phys.Name}");
    }

    private async Task WriteQuestionsAsync(
      QuestionsFullDto dto,
      CancellationToken token)
    {
      var phys = new QuestionsFull(Logger).DtoToPhysical(dto);

      phys.Id = 0;
      phys.ImageableType = scopeLevel;
      phys.ImageableId = parentId;

      await _dbContext.SystemQuestions.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"  imported question '{phys.Stem}'");

      foreach (var responseDto in dto.Responses)
        await ProcessScopedObjectQuestionResponseAsync(
          dto,
          responseDto,
          token);
    }

    private async Task ProcessScopedObjectQuestionResponseAsync(
      QuestionsFullDto questionDto,
      QuestionResponsesDto dto,
      CancellationToken token)
    {
      var phys = new QuestionResponses(Logger, questionDto).DtoToPhysical(dto);

      phys.Id = 0;

      await _dbContext.SystemQuestionResponses.AddAsync(phys);
      await _dbContext.SaveChangesAsync(token);

      Logger.LogInformation($"    imported question response '{phys.Response}'");

    }

  }
}
