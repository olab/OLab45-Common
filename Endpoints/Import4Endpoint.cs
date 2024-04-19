using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Endpoints;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.OLab4;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Endpoints;

public partial class Import4Endpoint : OLabEndpoint
{
  private readonly Importer _importer;

  public Import4Endpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context)
    : base(
        logger,
        configuration,
        context)
  {
  }

  public Import4Endpoint(
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

  public async Task<Maps> ImportAsync(
    IOLabAuthorization auth,
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token)
  {
    return await _importer.Import(
      auth,
      archiveFileStream,
      archiveFileName,
      token);
  }

  public async Task ExportAsync(
    IOLabAuthorization auth,
    Stream stream,
    uint mapId,
    CancellationToken token)
  {
    await _importer.ExportAsync(
      auth, 
      stream, 
      mapId, 
      token);
  }

  public async Task<MapsFullRelationsDto> ExportAsync(
    IOLabAuthorization auth,
    uint mapId,
    CancellationToken token)
  {
    return await _importer.ExportAsync(
      auth, 
      mapId, 
      token);
  }
}