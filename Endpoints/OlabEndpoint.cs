using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Constants = OLab.Api.Utils.Constants;
using OLab.Api.WikiTag;

namespace OLab.Api.Endpoints;

public class OLabEndpoint
{
  private readonly OLabDBContext _dbContext;
  public OLabDBContext GetDbContext() { return _dbContext; }

  private IOLabLogger _logger;
  public IOLabLogger GetLogger() { return _logger; }

  protected IOLabModuleProvider<IWikiTagModule> _wikiTagModules = null;
  public WikiTagModuleProvider GetWikiProvider() { return _wikiTagModules as WikiTagModuleProvider; }

  protected string token;
  protected IUserContext _userContext;
  protected readonly IOLabConfiguration _configuration;

  protected readonly IFileStorageModule _fileStorageModule;


  protected readonly MapNodesReaderWriter _nodesReaderWriter;
  protected readonly MapsReaderWriter _mapsReaderWriter;


  public OLabEndpoint(
    IOLabLogger logger,
    OLabDBContext context)
  {
    Guard.Argument(logger).NotNull(nameof(logger));
    Guard.Argument(context).NotNull(nameof(context));

    _dbContext = context;
    _logger = logger;

    _nodesReaderWriter = new MapNodesReaderWriter(GetLogger(), GetDbContext(), GetWikiProvider());
    _mapsReaderWriter = new MapsReaderWriter(GetLogger(), GetDbContext());

  }

  public OLabEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext) : this(logger, dbContext)
  {
    Guard.Argument(configuration).NotNull(nameof(configuration));

    _configuration = configuration;
  }

  public OLabEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : this(logger, configuration, dbContext)
  {
    Guard.Argument(wikiTagProvider).NotNull(nameof(wikiTagProvider));
    Guard.Argument(fileStorageProvider).NotNull(nameof(fileStorageProvider));

    var fileSystemModuleName = _configuration.GetAppSettings().FileStorageType;
    if (string.IsNullOrEmpty(fileSystemModuleName))
      throw new ConfigurationErrorsException($"missing FileStorageType");

    _fileStorageModule = fileStorageProvider.GetModule(fileSystemModuleName);
    _wikiTagModules = wikiTagProvider;
  }

  public void SetUserContext(IUserContext userContext)
  {
    _userContext = userContext;
  }

  //protected async ValueTask<Maps> GetMapAsync(uint id)
  //{
  //  var phys = await GetDbContext().Maps
  //    .Include("MapGroups")
  //    .FirstOrDefaultAsync(x => x.Id == id);
  //  if (phys != null)
  //    GetDbContext().Entry(phys).Collection(b => b.MapNodes).Load();

  //  return phys;
  //}

  /// <summary>
  /// Attach parent information to scoped object
  /// </summary>
  /// <param name="dto"></param>
  [NonAction]
  protected void AttachParentObject(ScopedObjectDto dto)
  {
    if (dto.ImageableType == Constants.ScopeLevelServer)
    {
      var obj = GetDbContext().Servers.FirstOrDefault(x => x.Id == dto.ImageableId);
      if (obj == null)
        throw new Exception($"orphaned object cannot find {dto.ImageableType} {dto.ImageableId}");
      dto.ParentInfo.Id = obj.Id;
      dto.ParentInfo.Name = obj.Name;
    }

    else if (dto.ImageableType == Constants.ScopeLevelMap)
    {
      var obj = GetDbContext().Maps.FirstOrDefault(x => x.Id == dto.ImageableId);
      if (obj == null)
        throw new Exception($"orphaned object cannot find {dto.ImageableType} {dto.ImageableId}");
      dto.ParentInfo.Id = obj.Id;
      dto.ParentInfo.Name = obj.Name;
    }

    else if (dto.ImageableType == Constants.ScopeLevelNode)
    {
      var obj = GetDbContext().MapNodes.FirstOrDefault(x => x.Id == dto.ImageableId);
      if (obj == null)
        throw new Exception($"orphaned object cannot find {dto.ImageableType} {dto.ImageableId}");
      dto.ParentInfo.Id = obj.Id;
      dto.ParentInfo.Name = obj.Title;
    }
  }

  protected IList<IdName> GetServerIdNames()
  {
    return GetDbContext().Servers
      .Select(x => new IdName() { Id = x.Id, Name = x.Name }).ToList();
  }

  protected IdName FindParentInfo(
    string scopeLevel,
    uint parentId,
    IList<IdName> maps,
    IList<IdName> nodes,
    IList<IdName> servers)
  {
    if (scopeLevel == Constants.ScopeLevelServer)
      return servers.FirstOrDefault(x => x.Id == parentId);

    if (scopeLevel == Constants.ScopeLevelMap)
      return maps.FirstOrDefault(x => x.Id == parentId);

    if (scopeLevel == Constants.ScopeLevelNode)
      return nodes.FirstOrDefault(x => x.Id == parentId);

    return null;
  }

  /// <summary>
  /// ReadAsync nodes for map
  /// </summary>
  /// <param name="map">Parent map to query for</param>
  /// <param name="enableWikiTanslation">PErform Name translation</param>
  /// <returns>List of mapnode dto's</returns>
  [NonAction]
  protected async Task<IList<MapNodesFullDto>> GetNodesAsync(Maps map, bool enableWikiTanslation = true)
  {
    var physList = await _nodesReaderWriter.GetByMapAsync(map.Id);

    // patch up any malformed nodes that have 0-by-0 size, 
    // which prevent them from being displayed
    var physNodes = physList.Where(x => (x.Height == 0) || (x.Width == 0)).ToList();
    foreach (var physNode in physNodes)
    {
      physNode.Height = 440;
      physNode.Width = 300;
    }

    var dtoList = new ObjectMapper.MapNodesFullMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTanslation).PhysicalToDto(physList);
    return dtoList;
  }

  /// <summary>
  /// ReadAsync node for map
  /// </summary>
  /// <param name="map">Map object</param>
  /// <param name="nodeId">Node id</param>
  /// <param name="hideHidden">flag to hide hidden links</param>
  /// <param name="enableWikiTranslation">PErform Name translation</param>
  /// <returns>MapsNodesFullRelationsDto</returns>
  [NonAction]
  protected async Task<MapsNodesFullRelationsDto> GetNodeAsync(
    uint mapId,
    uint nodeId,
    bool hideHidden,
    bool enableWikiTranslation)
  {
    var phys = await _nodesReaderWriter.GetNodeAsync(mapId, nodeId);
    if (phys == null)
    {
      GetLogger().LogError($"GetNodeSync unable to find map {mapId}, node {nodeId}");
      return new MapsNodesFullRelationsDto();
    }

    // explicitly load the related objects.
    GetDbContext().Entry(phys).Collection(b => b.MapNodeLinksNodeId1Navigation).Load();

    var builder = new ObjectMapper.MapsNodesFullRelationsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation);
    var dto = builder.PhysicalToDto(phys);

    var linkedIds =
      phys.MapNodeLinksNodeId1Navigation.Select(x => x.NodeId2).Distinct().ToList();

    var linkedNodes = await _nodesReaderWriter.GetNodesAsync(linkedIds);

    // add destination node title to link information
    foreach (var item in dto.MapNodeLinks)
    {
      var link = linkedNodes.Where(x => x.Id == item.DestinationId).FirstOrDefault();
      item.DestinationTitle = linkedNodes
        .Where(x => x.Id == item.DestinationId)
        .Select(x => x.Title).FirstOrDefault();

      if (string.IsNullOrEmpty(item.LinkText))
        item.LinkText = item.DestinationTitle;
    }

    // if asked for, remove any hidden links
    if (hideHidden)
    {
      GetLogger().LogInformation($"GetNodeSync hiding hidden links");
      dto.MapNodeLinks = dto.MapNodeLinks.Where(x => !x.IsHidden).ToList();
    }

    return dto;
  }

  /// <summary>
  /// ReadAsync a mapnode
  /// </summary>
  /// <param name="nodeId">Node id</param>
  /// <returns></returns>
  [NonAction]
  public async ValueTask<MapNodes> GetMapNodeAsync(uint nodeId)
  {
    var item = await _nodesReaderWriter.GetNodeAsync(nodeId);
    if (item == null)
      throw new OLabObjectNotFoundException("MapNodes", nodeId);

    // explicitly load the related objects.
    GetDbContext().Entry(item).Collection(b => b.MapNodeLinksNodeId1Navigation).Load();

    return item;
  }

  /// <summary>
  /// ReadAsync question response
  /// </summary>
  /// <param name="id">id</param>
  /// <returns></returns>
  [NonAction]
  protected async ValueTask<SystemQuestionResponses> GetQuestionResponseAsync(uint id)
  {
    var item = await GetDbContext().SystemQuestionResponses.FirstOrDefaultAsync(x => x.Id == id);

    if (item == null)
      throw new OLabObjectNotFoundException("SystemQuestionResponses", id);

    return item;
  }

  /// <summary>
  /// ReadAsync constant
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns></returns>
  [NonAction]
  protected async ValueTask<SystemConstants> GetConstantAsync(uint id)
  {
    var item = await GetDbContext().SystemConstants
      .FirstOrDefaultAsync(x => x.Id == id);

    if (item == null)
      throw new OLabObjectNotFoundException("SystemConstants", id);

    return item;
  }

  /// <summary>
  /// ReadAsync file
  /// </summary>
  /// <param name="id">file id</param>
  /// <returns></returns>
  [NonAction]
  protected async ValueTask<SystemFiles> GetFileAsync(uint id)
  {
    var item = await GetDbContext().SystemFiles
      .FirstOrDefaultAsync(x => x.Id == id);

    if (item == null)
      throw new OLabObjectNotFoundException("SystemFiles", id);

    return item;
  }

  /// <summary>
  /// ReadAsync question
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns></returns>
  [NonAction]
  protected async ValueTask<SystemQuestions> GetQuestionSimpleAsync(uint id)
  {
    var item = await GetDbContext().SystemQuestions
      .FirstOrDefaultAsync(x => x.Id == id);

    if (item == null)
      throw new OLabObjectNotFoundException("SystemQuestions", id);

    return item;
  }

  /// <summary>
  /// ReadAsync question with responses
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns></returns>
  [NonAction]
  protected async ValueTask<SystemQuestions> GetQuestionAsync(uint id)
  {
    var item = await GetDbContext().SystemQuestions
      .Include(x => x.SystemQuestionResponses)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (item == null)
      throw new OLabObjectNotFoundException("SystemQuestions", id);

    return item;
  }

  /// <summary>
  /// ReadAsync counter 
  /// </summary>
  /// <param name="id">counter id</param>
  /// <returns>Counter</returns>
  [NonAction]
  protected async Task<SystemCounters> GetCounterAsync(uint id)
  {
    var phys = await GetDbContext().SystemCounters.SingleOrDefaultAsync(x => x.Id == id);
    if (phys != null)
    {
      if (phys.Value == null)
        phys.Value = new List<byte>().ToArray();
      if (phys.StartValue == null)
        phys.StartValue = new List<byte>().ToArray();
    }

    return phys;
  }

}