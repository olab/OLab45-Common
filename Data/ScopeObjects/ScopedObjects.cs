using Humanizer;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;

namespace OLab.Data;

public partial class ScopedObjects
{
  private readonly IDictionary<uint, uint> _counterIds = new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _nodeIds = new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _questionIds = new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _mapIds = new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _fileIds = new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _constantIds = new Dictionary<uint, uint>();

  private IOLabLogger _logger { get; }
  protected IOLabLogger GetLogger() { return _logger; }
  private OLabDBContext _dbContext { get; set; }
  protected OLabDBContext GetDbContext() { return _dbContext; }

  protected IOLabModuleProvider<IWikiTagModule> _wikiTagModules = null;
  public WikiTagModuleProvider GetWikiProvider() { return _wikiTagModules as WikiTagModuleProvider; }

  public uint ScopeId { get; private set; }
  public string ScopeLevel { get; private set; }

  public List<SystemConstants> ConstantsPhys { get; set; }
  public List<SystemCounters> CountersPhys { get; set; }
  public List<SystemCounterActions> CounterActionsPhys { get; set; }
  public List<SystemFiles> FilesPhys { get; set; }
  public List<SystemQuestions> QuestionsPhys { get; set; }
  public List<SystemScripts> ScriptsPhys { get; set; }
  public List<SystemThemes> ThemesPhys { get; set; }

  private readonly IFileStorageModule _fileStorageModule;

  public ScopedObjects(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    IFileStorageModule fileStorageModule = null) : this()
  {
    _logger = logger;
    _dbContext = dbContext;
    _fileStorageModule = fileStorageModule;
    _wikiTagModules = tagProvider;
  }

  /// <summary>
  /// Default ctor needed for Mapping
  /// </summary>
  public ScopedObjects()
  {
    ConstantsPhys = new List<SystemConstants>();
    CountersPhys = new List<SystemCounters>();
    CounterActionsPhys = new List<SystemCounterActions>();
    QuestionsPhys = new List<SystemQuestions>();
    FilesPhys = new List<SystemFiles>();
    ScriptsPhys = new List<SystemScripts>();
    ThemesPhys = new List<SystemThemes>();

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

  public void AddMapIdCrossReference(uint from, uint to) { _mapIds.Add( from, to ); }
  public void AddCounterIdCrossReference(uint from, uint to) { _counterIds.Add( from, to ); }
  public void AddMapNodeIdCrossReference(uint from, uint to) { _nodeIds.Add( from, to ); }
  public void AddQuestionIdCrossReference(uint from, uint to) { _questionIds.Add( from, to ); }
  public void AddConstantIdCrossReference(uint from, uint to) { _constantIds.Add( from, to ); }
  public void AddFileIdCrossReference(uint from, uint to) { _fileIds.Add( from, to ); }

  public uint GetMapIdCrossReference(uint from) { return _mapIds[ from ]; }
  public uint GetMapNodeIdCrossReference(uint from) { return _nodeIds[ from ]; }
  public uint GetQuestionIdCrossReference(uint from) { return _questionIds[ from ]; }
  public uint GetCounterIdCrossReference(uint from) { return _counterIds[ from ]; }
  public uint GetConstantIdCrossReference(uint from) { return _constantIds[ from ]; }
  public uint GetFileIdCrossReference(uint from) { return _fileIds[ from ]; }

  /// <summary>
  /// Appends a ScopedObjectsMapper to the current one
  /// </summary>
  /// <param name="source">Source ScopedObjectsMapper</param>
  public void Combine(ScopedObjects source)
  {
    ConstantsPhys.AddRange( source.ConstantsPhys );
    CountersPhys.AddRange( source.CountersPhys );
    CounterActionsPhys.AddRange( source.CounterActionsPhys );
    QuestionsPhys.AddRange( source.QuestionsPhys );
    FilesPhys.AddRange( source.FilesPhys );
    ScriptsPhys.AddRange( source.ScriptsPhys );
    ThemesPhys.AddRange( source.ThemesPhys );
  }
}
