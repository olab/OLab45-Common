using OLab.Access.Interfaces;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interface;
using System.Collections.Generic;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  public const string MapFileName = "map.json";

  protected IOLabLogger _logger;
  public IOLabLogger GetLogger() { return _logger; }

  protected IOLabModuleProvider<IWikiTagModule> _wikiTagModules = null;
  public WikiTagModuleProvider GetWikiProvider() { return _wikiTagModules as WikiTagModuleProvider; }

  private readonly OLabDBContext _dbContext;
  public OLabDBContext GetDbContext() { return _dbContext; }

  public IOLabAuthorization Authorization { get; set; }
  private readonly IOLabConfiguration _configuration;
  private readonly IFileStorageModule _fileModule;
  private readonly IDictionary<uint, uint?> _nodeIdTranslation = new Dictionary<uint, uint?>();

  public IFileStorageModule GetFileStorageModule() { return _fileModule; }
  public IOLabConfiguration GetConfiguration() { return _configuration; }
  public AppSettings Settings() { return _configuration.GetAppSettings(); }

  public Importer(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagModules,
    IFileStorageModule fileStorageModule)
  {
    _dbContext = dbContext;
    _logger = logger;

    _configuration = configuration;

    _wikiTagModules = wikiTagModules;
    _fileModule = fileStorageModule;

  }

}