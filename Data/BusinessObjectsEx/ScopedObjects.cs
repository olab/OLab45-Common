using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OLab.Api.Model;
using OLab.Data.Interface;
using DocumentFormat.OpenXml.InkML;
using System.Linq;
using System;
using Dawn;
using DocumentFormat.OpenXml.Drawing;
using Humanizer;
using OLab.Api.Common;
using OLab.Api.Dto;
using DocumentFormat.OpenXml.EMMA;
using OLab.Common.Interfaces;

namespace OLab.Data.BusinessObjects
{
  public partial class ScopedObjects
  {
    public IOLabLogger Logger { get; }
    #region Properties

    public OLabDBContext dbContext { get; set; }
    public uint parentId { get; }
    public string scopeLevel { get; }

    public List<SystemConstants> Constants { get; set; }
    public List<SystemCounters> Counters { get; set; }
    public List<SystemCounterActions> CounterActions { get; set; }
    public List<SystemFiles> Files { get; set; }
    public List<SystemQuestions> Questions { get; set; }
    public List<SystemScripts> Scripts { get; set; }
    public List<SystemThemes> Themes { get; set; }

    private IDictionary<string, ScopedObjects> scopedObjectList = new Dictionary<string, ScopedObjects>();

    #endregion

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
      uint? nodeId)
    {
      Logger = logger;
      this.dbContext = dbContext;

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
      string scopeLevel)
    {
      Logger = logger;
      this.dbContext = dbContext;
      this.parentId = parentId;
      this.scopeLevel = scopeLevel;

      scopedObjectList.Add(
        new KeyValuePair<string, ScopedObjects>(
          scopeLevel,
          new ScopedObjects(Logger, dbContext, parentId, scopeLevel)));
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
    /// Appends a ScopedObjects to the current one
    /// </summary>
    /// <param name="source">Source ScopedObjects</param>
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
    public async Task<List<SystemConstants>> GetScopedConstantsAsync()
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));
      var items = new List<SystemConstants>();

      items.AddRange(await dbContext.SystemConstants.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      return items;
    }

    /// <summary>
    /// Gets question scoped objects
    /// </summary>
    /// <returns></returns>
    public async Task<List<SystemQuestions>> GetScopedQuestionsAsync()
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));
      var items = new List<SystemQuestions>();

      items.AddRange(await dbContext.SystemQuestions
        .Where(x => x.ImageableType == scopeLevel && x.ImageableId == parentId)
        .Include("SystemQuestionResponses")
        .ToListAsync());

      // order the responses by Order field
      foreach (var item in items)
        item.SystemQuestionResponses = item.SystemQuestionResponses.OrderBy(x => x.Order).ToList();

      return items;
    }

    /// <summary>
    /// Gets file scoped objects
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns></returns>
    public async Task<List<SystemFiles>> GetScopedFilesAsync(
      IFileStorageModule fileStorageModule = null)
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));

      // if no file module, return empty list
      if (fileStorageModule == null)
        return new List<SystemFiles>();

      var items = await dbContext.SystemFiles.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync();

      // ask the module to add appropriate URLs to files
      fileStorageModule.AttachUrls(items);

      return items;
    }

    /// <summary>
    /// Gets thems scoped objects
    /// </summary>
    /// <returns></returns>
    public async Task<List<SystemThemes>> GetScopedThemesAsync()
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));
      var items = new List<SystemThemes>();

      items.AddRange(await dbContext.SystemThemes.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      return items;
    }

    /// <summary>
    /// Gets scriptscoped objects
    /// </summary>
    /// <returns></returns>
    public async Task<List<SystemScripts>> GetScopedScriptsAsync()
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));
      var items = new List<SystemScripts>();

      items.AddRange(await dbContext.SystemScripts.Where(x =>
        x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

      return items;
    }

    /// <summary>
    /// GetAsync counters associated with a 'parent' object 
    /// </summary>
    /// <param name="sinceTime">(optional) looks for values changed since a (unix) time</param>
    /// <returns>List of counters</returns>
    public async Task<List<SystemCounters>> GetScopedCountersAsync(uint sinceTime = 0)
    {
      Guard.Argument(dbContext).NotNull(nameof(dbContext));
      var items = new List<SystemCounters>();

      if (sinceTime != 0)
      {
        // generate DateTime from sinceTime
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(sinceTime).ToLocalTime();
        items.AddRange(await dbContext.SystemCounters.Where(x =>
          x.ImageableType == scopeLevel && x.ImageableId == parentId && x.UpdatedAt >= dateTime).ToListAsync());
      }
      else
      {
        items.AddRange(await dbContext.SystemCounters.Where(x =>
          x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());
      }

      return items;
    }

    private async ValueTask<ScopedObjects> GetInternalAsync(
      IFileStorageModule fileStorageModule = null)
    {
      var phys = new ScopedObjects
      {
        Constants = await GetScopedConstantsAsync(),
        Questions = await GetScopedQuestionsAsync(),
        Files = await GetScopedFilesAsync(fileStorageModule),
        Scripts = await GetScopedScriptsAsync(),
        Themes = await GetScopedThemesAsync(),
        Counters = await GetScopedCountersAsync(0)
      };

      if (scopeLevel == Api.Utils.Constants.ScopeLevelMap)
      {
        var items = new List<SystemCounterActions>();
        items.AddRange(await dbContext.SystemCounterActions.Where(x =>
            x.MapId == parentId).ToListAsync());

        phys.CounterActions.AddRange(items);
      }

      return phys;

    }

    /// <summary>
    /// GetAsync scoped objects for a specific scope level
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns>ScopedObject dto</returns>
    public async ValueTask<ScopedObjects> GetAsync(
      string scopeLevel,
      IFileStorageModule fileStorageModule = null)
    {
      return await scopedObjectList[scopeLevel].GetInternalAsync(fileStorageModule);
    }

    /// <summary>
    /// GetAsync scoped objects for all levels
    /// </summary>
    /// <param name="fileStorageModule">File storage module that provides URL</param>
    /// <returns>ScopedObject dto</returns>
    public async ValueTask<ScopedObjects> GetAsync(
      IFileStorageModule fileStorageModule = null)
    {
      var phys = new ScopedObjects();
      foreach (var item in scopedObjectList)
      {
        var typePhys = await GetAsync(item.Key, fileStorageModule);
        phys.Combine(typePhys);
      }

      return phys;
    }

  }
}
