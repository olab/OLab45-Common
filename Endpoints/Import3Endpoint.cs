using OLab.Api.Endpoints;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.OLab3;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Endpoints;

public partial class Import3Endpoint : OLabEndpoint
{
  private readonly Importer _importer;

  public Import3Endpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context)
    : base(
        logger,
        configuration,
        context)
  {
  }

  public Import3Endpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider)
    : base(
        logger,
        configuration,
        context,
        wikiTagProvider,
        fileStorageProvider)
  {
    _importer = new Importer(
      Logger,
      _configuration,
      dbContext,
      _wikiTagProvider,
      _fileStorageModule);
  }

  public async Task<uint> ImportAsync(
    Stream archvieFileStream,
    string archiveFileName,
    CancellationToken token)
  {
    uint mapId = await _importer.Import(archvieFileStream, archiveFileName, token);
    return mapId;
  }

}