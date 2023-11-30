using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data.Interface;
using OLab.Import.Interface;
using OLab.Import.OLab3.Dtos;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab3;

public class Importer : IImporter
{
  public enum DtoTypes
  {
    XmlMapDto,
    XmlMapNodeDto,
    XmlMapNodeLinkDto,
    XmlMapCounterDto,
    XmlMapCounterRuleDto,
    XmlMapElementDto,
    XmlMapNodeCounterDto,
    XmlMapQuestionDto,
    XmlMapQuestionResponseDto,
    XmlMapNodeSectionDto,
    XmlMapNodeSectionNodeDto,
    XmlMapVpdDto,
    XmlMapVpdElementDto,
    XmlMediaElementsDto,
    XmlMetadataDto,
    XmlManifestDto,
    XmlMapAvatarDto
  };

  private readonly OLabDBContext _dbContext;
  public readonly AppSettings _appSettings;
  private readonly IOLabConfiguration _configuration;
  private readonly IDictionary<DtoTypes, XmlDto> _dtos = new Dictionary<DtoTypes, XmlDto>();
  private readonly IOLabLogger Logger;
  private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;
  public readonly IFileStorageModule FileStorageModule;

  public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _wikiTagProvider; }
  public IFileStorageModule GetFileStorageModule() { return FileStorageModule; }
  public XmlDto GetDto(DtoTypes type) { return _dtos[type]; }
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

    XmlDto dto = new XmlMapDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapElementDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMediaElementsDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapAvatarDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapVpdDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapVpdElementDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapCounterDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapQuestionDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapQuestionResponseDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeCounterDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeLinkDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapCounterRuleDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    // dto = new XmlMapNodeSectionDto(Logger, this);
    // _dtos.Add(dto.DtoType, dto);

    dto = new XmlMetadataDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlManifestDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

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
  /// <param name="stream">Stream to write file to</param>
  /// <param name="fileName">Export ZIP file name</param>
  /// <returns>true</returns>
  public async Task<bool> ProcessImportFileAsync(
    Stream stream,
    string fileName,
    CancellationToken token = default)
  {
    var importStatus = true;

    try
    {
      Logger.LogInformation($"Module archive file: {FileStorageModule.BuildPath(OLabFileStorageModule.ImportRoot, fileName)}");

      await FileStorageModule.WriteFileAsync(
        stream, 
        FileStorageModule.BuildPath(OLabFileStorageModule.ImportRoot, fileName), 
        fileName, 
        token);

      var extractFolderName = Path.GetFileNameWithoutExtension(fileName);
      Logger.LogInformation($"Folder extract directory: {extractFolderName}");

      await FileStorageModule.ExtractFileToStorageAsync(
        OLabFileStorageModule.ImportRoot,
        fileName,
        extractFolderName,
        token);

      foreach (var dto in _dtos.Values)
        dto.Load(FileStorageModule.BuildPath(OLabFileStorageModule.ImportRoot, extractFolderName));

      // delete source import file
      await GetFileStorageModule().DeleteFileAsync(
        OLabFileStorageModule.ImportRoot, 
        fileName);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, $"Read error: {ex.Message}");
      throw;
    }

    return importStatus;
  }

  /// <summary>
  /// WRite import dtos to database
  /// </summary>
  /// <returns>true</returns>
  public bool WriteImportToDatabase()
  {
    var rc = true;

    using (var transaction = _dbContext.Database.BeginTransaction())
    {
      try
      {

        foreach (var dto in _dtos.Values)
          dto.Save();

        transaction.Commit();

        var xmlMapDto = _dtos[DtoTypes.XmlMapDto] as XmlMapDto;
        var xmlMap = (XmlMap)xmlMapDto.GetDbPhys();
        Logger.LogInformation($"Loaded map '{xmlMap.Data[0].Name}'. id = {xmlMap.Data[0].Id}");
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Error saving import. reason: {ex.Message} ");
        transaction.Rollback();
      }

    }

    return rc;
  }

  public Task Export(Stream stream, uint mapId, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }
}