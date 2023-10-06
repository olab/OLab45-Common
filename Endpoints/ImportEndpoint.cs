using Dawn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Endpoints;
using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Api.Model.ReaderWriter;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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

  public async Task<bool> Import(
    IFormFile file, 
    CancellationToken token)
  {
    var stream = new MemoryStream();
    file.CopyTo(stream);

    // need to set the stream position to start
    stream.Position = 0;

    // save import file to persistent storage
    var archiveFile = 
      await _fileStorageModule.UploadFileAsync(Logger, stream, file.FileName, token);

    Logger.LogInformation($"Importing archive: '{archiveFile}'");

    if (await _importer.ProcessImportFileAsync(archiveFile, token))
      _importer.WriteImportToDatabase();

    // delete source import file
    await _fileStorageModule.DeleteFileAsync(Logger, archiveFile);

    return true;
  }

}