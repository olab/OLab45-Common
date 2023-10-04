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
    _importer = new Importer(Logger, _configuration, dbContext, _wikiTagProvider);
  }

  public async Task<bool> Import(IFormFile file, CancellationToken cancellationToken)
  {
    var length = file.Length;
    var stream = new MemoryStream();
    file.CopyTo(stream);

    // need to set the stream position to start
    stream.Position = 0;

    var newFileName = await _fileStorageModule.UploadFile(Logger, stream, file.FileName, cancellationToken);

    Logger.LogInformation($"Loading archive: '{newFileName}'");

    if (_importer.LoadAll(newFileName))
      _importer.SaveAll();

    return true;
  }

}