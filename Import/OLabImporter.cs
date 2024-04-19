using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interface;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data;
public abstract class OLabImporter : IImporter
{
  private readonly OLabDBContext _dbContext;
  private readonly IOLabConfiguration _configuration;
  protected readonly IOLabLogger Logger;
  public IOLabAuthorization Authorization { get; set; }
  private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;
  private readonly IFileStorageModule _fileStorageModule;

  public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _wikiTagProvider; }
  public IFileStorageModule GetFileModule() { return _fileStorageModule; }
  public OLabDBContext GetDbContext() { return _dbContext; }
  public IOLabConfiguration GetConfiguration() { return _configuration; }
  public AppSettings Settings() { return _configuration.GetAppSettings(); }

  protected OLabImporter(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IFileStorageModule fileStorageModule)
  {
    _dbContext = context;
    _configuration = configuration;

    Logger = logger;

    _wikiTagProvider = wikiTagProvider;
    _fileStorageModule = fileStorageModule;
  }

  public abstract Task<Maps> Import(IOLabAuthorization auth, Stream stream, string fileName, CancellationToken token = default);

  public abstract Task ExportAsync(Stream stream, uint mapId, CancellationToken token = default);

  public abstract Task<MapsFullRelationsDto> ExportAsync(uint mapId, CancellationToken token = default);
}
