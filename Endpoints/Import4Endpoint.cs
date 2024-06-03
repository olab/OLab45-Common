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
      GetLogger(),
      _configuration,
      GetDbContext(),
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
    Stream stream,
    uint mapId,
    CancellationToken token)
  {
    await _importer.ExportAsync(stream, mapId, token);
  }

  public async Task<MapsFullRelationsDto> ExportAsync(
    uint mapId,
    CancellationToken token)
  {
    return await _importer.ExportAsync(mapId, token);
  }
}