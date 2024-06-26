using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interface;
using System.Collections.Generic;

namespace OLab.Import.OLab4;

public partial class Importer : IImporter
{
  public const string MapFileName = "map.json";

  public IOLabAuthorization Authorization { get; set; }
  private readonly OLabDBContext _dbContext;
  private readonly IOLabConfiguration _configuration;
  private readonly IOLabLogger Logger;
  private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;
  private readonly IFileStorageModule _fileModule;
  private readonly IDictionary<uint, uint?> _nodeIdTranslation = new Dictionary<uint, uint?>();

  public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _wikiTagProvider; }
  public IFileStorageModule GetFileStorageModule() { return _fileModule; }
  public OLabDBContext GetDbContext() { return _dbContext; }
  public IOLabConfiguration GetConfiguration() { return _configuration; }
  public AppSettings Settings() { return _configuration.GetAppSettings(); }

  public Importer(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IFileStorageModule fileStorageModule)
  {
    _dbContext = context;
    _configuration = configuration;

    Logger = logger; // OLabLogger.CreateNew<Importer>(logger, true);

    _wikiTagProvider = wikiTagProvider;
    _fileModule = fileStorageModule;

  }

}