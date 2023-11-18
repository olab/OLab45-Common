using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using OLab.Api.Endpoints;
using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Endpoints;

public partial class ImportEndpoint : OLabEndpoint
{
  private readonly IImporter _importer;

  public ImportEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context)
    : base(
        logger,
        configuration,
        context)
  {
  }

  public ImportEndpoint(
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

  public async Task<bool> ImportAsync(
    Stream archvieFileStream,
    string archiveFileName,
    CancellationToken token)
  {
    await _importer.Import(archvieFileStream, archiveFileName, token);
    return true;
  }

}