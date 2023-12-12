using AutoMapper.Internal.Mappers;
using Dawn;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HeyRed.Mime;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data;

public partial class ScopedObjects
{
  public IOLabLogger Logger { get; }
  private readonly IDictionary<uint, uint> _counterIds =
    new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _nodeIds =
    new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _questionIds =
    new Dictionary<uint, uint>();
  private readonly IDictionary<uint, uint> _mapIds =
    new Dictionary<uint, uint>();

  private OLabDBContext _dbContext { get; set; }

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
    IFileStorageModule fileStorageModule = null) : this()
  {
    Logger = logger;
    _dbContext = dbContext;
    _fileStorageModule = fileStorageModule;
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

  public void AddMapIdCrossReference(uint from, uint to) { _mapIds.Add(from, to); }
  public void AddCounterIdCrossReference(uint from, uint to) { _counterIds.Add(from, to); }
  public void AddMapNodeIdCrossReference(uint from, uint to) { _nodeIds.Add(from, to); }
  public void AddQuestionIdCrossReference(uint from, uint to) { _questionIds.Add(from, to); }

  public uint GetMapIdCrossReference(uint from) { return _mapIds[from]; }
  public uint GetMapNodeIdCrossReference(uint from) { return _nodeIds[from]; }
  public uint GetQuestionIdCrossReference(uint from) { return _questionIds[from]; }
  public uint GetCounterIdCrossReference(uint from) { return _counterIds[from]; }

  /// <summary>
  /// Appends a ScopedObjectsMapper to the current one
  /// </summary>
  /// <param name="source">Source ScopedObjectsMapper</param>
  public void Combine(ScopedObjects source)
  {
    ConstantsPhys.AddRange(source.ConstantsPhys);
    CountersPhys.AddRange(source.CountersPhys);
    CounterActionsPhys.AddRange(source.CounterActionsPhys);
    QuestionsPhys.AddRange(source.QuestionsPhys);
    FilesPhys.AddRange(source.FilesPhys);
    ScriptsPhys.AddRange(source.ScriptsPhys);
    ThemesPhys.AddRange(source.ThemesPhys);
  }
}
