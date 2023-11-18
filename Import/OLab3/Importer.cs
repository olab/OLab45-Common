using OLab.Api.Model;
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

    private readonly OLabDBContext _context;
    public readonly AppSettings _appSettings;
    private readonly IOLabConfiguration _configuration;
    private readonly IDictionary<DtoTypes, XmlDto> _dtos = new Dictionary<DtoTypes, XmlDto>();
    private readonly IOLabLogger Logger;
    private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;
    public readonly IFileStorageModule FileStorageModule;

    public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _wikiTagProvider; }
    public IFileStorageModule GetFileStorageModule() { return FileStorageModule; }
    public XmlDto GetDto(DtoTypes type) { return _dtos[type]; }
    public OLabDBContext GetContext() { return _context; }
    public IOLabConfiguration GetConfiguration() { return _configuration; }
    public AppSettings Settings() { return _appSettings; }

    private string _extractPath;

    public Importer(
      IOLabLogger logger,
      IOLabConfiguration configuration,
      OLabDBContext context,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
      IFileStorageModule fileStorageModule)
    {
        _appSettings = configuration.GetAppSettings();
        _context = context;
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

    public void LogInformation(string message)
    {
        Logger.LogInformation(message);
    }

    public void LogDebug(string message)
    {
        Logger.LogInformation(message);
    }

    public void LogWarning(string message)
    {
        Logger.LogWarning(message);
    }

    public void LogError(Exception ex, string message)
    {
        Logger.LogError(ex, message);
    }

    public void LogError(string message)
    {
        Logger.LogError(message);
    }

    private string Extract(string archiveFileName)
    {
        var tempDir = Path.GetTempPath();
        // Logger.LogInformation($"Import temporary directory '{tempDir}'");

        _extractPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(archiveFileName));

        // Logger.LogInformation($"Import extract directory '{_extractPath}'");

        if (Directory.Exists(_extractPath))
        {
            Logger.LogInformation($"Deleting existing extract directory");
            Directory.Delete(_extractPath, true);
        }
        else
        {
            _ = Directory.CreateDirectory(_extractPath);
        }

        ZipFile.ExtractToDirectory(archiveFileName, _extractPath);

        return _extractPath;
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

            foreach (var dto in _dtos.Values)
                dto.Load($"{_configuration.GetAppSettings().FileImportFolder}{GetFileStorageModule().GetFolderSeparator()}{extractFolderName}");

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

        using (var transaction = _context.Database.BeginTransaction())
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

}