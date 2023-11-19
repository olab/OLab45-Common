using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Import.Interface;
using OLab.Import.OLab3.Dtos;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab4;

public class Importer : IImporter
{
  private readonly OLabDBContext _dbContext;
  public readonly AppSettings _appSettings;
  private readonly IOLabConfiguration _configuration;
  private readonly IOLabLogger Logger;
  private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;
  public readonly IFileStorageModule FileStorageModule;

  public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _wikiTagProvider; }
  public IFileStorageModule GetFileStorageModule() { return FileStorageModule; }
  public OLabDBContext GetDbContext() { return _dbContext; }
  public IOLabConfiguration GetConfiguration() { return _configuration; }
  public AppSettings Settings() { return _appSettings; }

  public Importer(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IFileStorageModule fileStorageModule)
  {
    _appSettings = configuration.GetAppSettings();
    _dbContext = context;
    _configuration = configuration;

    Logger = OLabLogger.CreateNew<Importer>(logger);

    _wikiTagProvider = wikiTagProvider;
    FileStorageModule = fileStorageModule;

  }

  public async Task Import(
    Stream archiveFileStream,
    string archiveFileName,
    CancellationToken token = default)
  {
    if (await ProcessImportFileAsync(archiveFileStream, archiveFileName, token))
      WriteImportToDatabase();
  }

  /// <summary>
  /// Loads import xml files into memory
  /// </summary>
  /// <param name="importFileName">Export ZIP file name</param>
  /// <returns>true</returns>
  public async Task<bool> ProcessImportFileAsync(
    Stream importFileStream,
    string importFileName,
    CancellationToken token = default)
  {
    var importStatus = true;

    try
    {
      var moduleArchiveFile = $"{_configuration.GetAppSettings().FileImportFolder}{GetFileStorageModule().GetFolderSeparator()}{importFileName}";
      Logger.LogInformation($"Module archive file: {moduleArchiveFile}");

      await FileStorageModule.CopyFiletoStreamAsync(importFileStream, moduleArchiveFile, token);

      var extractFolderName = Path.GetFileNameWithoutExtension(importFileName).Replace(".", "");
      Logger.LogInformation($"Folder extract directory: {extractFolderName}");

      await FileStorageModule.ExtractFileToStorageAsync(
        _configuration.GetAppSettings().FileImportFolder,
        importFileName,
        extractFolderName,
        token);

      //foreach (var dto in _dtos.Values)
      //  dto.Load($"{_configuration.GetAppSettings().FileImportFolder}{GetFileStorageModule().GetFolderSeparator()}{extractFolderName}");

      // delete source import file
      await GetFileStorageModule().DeleteFileAsync(
        Path.GetDirectoryName(importFileName),
        Path.GetFileName(importFileName));

    }
    catch (Exception ex)
    {
      Logger.LogError(ex, $"Load error: {ex.Message}");
      throw;
    }

    return importStatus;
  }

  public bool WriteImportToDatabase()
  {
    var rc = true;

    using (var transaction = _dbContext.Database.BeginTransaction())
      try
      {

        //foreach (var dto in _dtos.Values)
        //  dto.Save();

        //transaction.Commit();

        //var xmlMapDto = _dtos[DtoTypes.XmlMapDto] as XmlMapDto;
        //var xmlMap = (XmlMap)xmlMapDto.GetDbPhys();
        //Logger.LogInformation($"Loaded map '{xmlMap.Data[0].Name}'. id = {xmlMap.Data[0].Id}");
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Error saving import. reason: {ex.Message} ");
        transaction.Rollback();
      }

    return rc;
  }

  public async Task Export(Stream stream, uint mapId, CancellationToken token = default)
  {
    Logger.LogInformation($"Exporting mapId: {mapId} ");

    var map = await _dbContext.Maps
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Id == mapId,
        token);

    MapsFullRelationsDto dto = new MapsFullRelationsMapper(
      Logger,
      _wikiTagProvider as WikiTagProvider
    ).PhysicalToDto(map);

    var mapQuestions = _dbContext.SystemQuestions.Where( x => x.ImageableId == mapId && x.ImageableType == OLab.Api.Utils.Constants.ScopeLevelMap).ToList();
    var mapConstants = _dbContext.SystemConstants.Where( x => x.ImageableId == mapId && x.ImageableType == OLab.Api.Utils.Constants.ScopeLevelMap).ToList();
    var mapCounters = _dbContext.SystemCounters.Where( x => x.ImageableId == mapId && x.ImageableType == OLab.Api.Utils.Constants.ScopeLevelMap).ToList();
    var mapFiles = _dbContext.SystemFiles.Where( x => x.ImageableId == mapId && x.ImageableType == OLab.Api.Utils.Constants.ScopeLevelMap).ToList();


    var rawJson = JsonConvert.SerializeObject(dto);

    using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
    {
      var entry = zipArchive.CreateEntry("map.json");
      using (var fileContentStream = new MemoryStream())
      {
        var writer = new StreamWriter(fileContentStream);
        writer.Write(rawJson);
        writer.Flush();

        fileContentStream.Position = 0;

        Logger.LogInformation($"Read map: {map.Name} into stream. json size = {fileContentStream.Length} ");

        using (var entryStream = entry.Open())
        {
          fileContentStream.CopyTo(entryStream);
        }
      }
    }
  }
}