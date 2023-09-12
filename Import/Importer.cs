using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace OLab.Api.Importer
{
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
      // XmlMapNodeSectionDto,
      // XmlMapNodeSectionNodeDto,
      XmlMapVpdDto,
      XmlMapVpdElementDto,
      XmlMediaElementsDto,
      XmlMetadataDto,
      XmlManifestDto,
      XmlMapAvatarDto
    };

    private readonly OLabDBContext _context;
    public readonly AppSettings _settings;
    private readonly IDictionary<DtoTypes, XmlDto> _dtos = new Dictionary<DtoTypes, XmlDto>();
    private readonly IOLabLogger Logger;
    private readonly WikiTagProvider _tagProvider;

    public WikiTagProvider GetWikiProvider() { return _tagProvider; }
    public XmlDto GetDto(DtoTypes type) { return _dtos[type]; }
    public OLabDBContext GetContext() { return _context; }
    public IOLabLogger GetLogger() { return Logger; }
    public AppSettings Settings() { return _settings; }

    private string _logFileDirectory;
    private string _extractPath;
    private string _importFileName;

    public Importer(
      IOLabLogger logger,
      IOptions<AppSettings> appSettings,
      OLabDBContext context)
    {
      _settings = appSettings.Value;
      _context = context;

      Logger = OLabLogger.CreateNew<Importer>(logger);

      _tagProvider = new WikiTagProvider(Logger);

      XmlDto dto = new XmlMapDto(this);
      _dtos.Add(DtoTypes.XmlMapDto, dto);

      dto = new XmlMapElementDto(this);
      _dtos.Add(DtoTypes.XmlMapElementDto, dto);

      dto = new XmlMediaElementsDto(this);
      _dtos.Add(DtoTypes.XmlMediaElementsDto, dto);

      dto = new XmlMapAvatarDto(this);
      _dtos.Add(DtoTypes.XmlMapAvatarDto, dto);

      dto = new XmlMapVpdDto(this);
      _dtos.Add(DtoTypes.XmlMapVpdDto, dto);

      dto = new XmlMapVpdElementDto(this);
      _dtos.Add(DtoTypes.XmlMapVpdElementDto, dto);

      dto = new XmlMapCounterDto(this);
      _dtos.Add(DtoTypes.XmlMapCounterDto, dto);

      dto = new XmlMapQuestionDto(this);
      _dtos.Add(DtoTypes.XmlMapQuestionDto, dto);

      dto = new XmlMapQuestionResponseDto(this);
      _dtos.Add(DtoTypes.XmlMapQuestionResponseDto, dto);

      dto = new XmlMapNodeDto(this);
      _dtos.Add(DtoTypes.XmlMapNodeDto, dto);

      dto = new XmlMapNodeCounterDto(this);
      _dtos.Add(DtoTypes.XmlMapNodeCounterDto, dto);

      dto = new XmlMapNodeLinkDto(this);
      _dtos.Add(DtoTypes.XmlMapNodeLinkDto, dto);

      dto = new XmlMapCounterRuleDto(this);
      _dtos.Add(DtoTypes.XmlMapCounterRuleDto, dto);

      // dto = new XmlMapNodeSectionDto(this);
      // _dtos.Add(DtoTypes.XmlMapNodeSectionDto, dto);

      // dto = new XmlMapNodeSectionNodeDto(this);
      // _dtos.Add(DtoTypes.XmlMapNodeSectionNodeDto, dto);

      dto = new XmlMetadataDto(this);
      _dtos.Add(DtoTypes.XmlMetadataDto, dto);

      dto = new XmlManifestDto(this);
      _dtos.Add(DtoTypes.XmlManifestDto, dto);

    }

    public void LogInformation(string message)
    {
      Logger.LogInformation(message);
    }

    public void LogDebug(string message)
    {
      Logger.LogDebug(message);
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
      // Logger.LogDebug($"Import temporary directory '{tempDir}'");

      _logFileDirectory = Path.GetDirectoryName(archiveFileName);
      _extractPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(archiveFileName));
      _importFileName = Path.GetFileNameWithoutExtension(archiveFileName);

      // Logger.LogDebug($"Import extract directory '{_extractPath}'");

      if (Directory.Exists(_extractPath))
      {
        Logger.LogDebug($"Deleting existing extract directory");
        Directory.Delete(_extractPath, true);
      }
      else
      {
        _ = Directory.CreateDirectory(_extractPath);
      }

      System.IO.Compression.ZipFile.ExtractToDirectory(archiveFileName, _extractPath);

      return _extractPath;
    }

    /// <summary>
    /// Loads import xml files into memory
    /// </summary>
    /// <param name="archiveFileName">Export ZIP file name</param>
    /// <returns>true</returns>
    public bool LoadAll(string archiveFileName)
    {
      var importStatus = true;

      try
      {
        var extractPath = Extract(archiveFileName);

        foreach (var dto in _dtos.Values)
          dto.Load(extractPath);

      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Load error: {ex.Message}");
      }

      return importStatus;
    }

    public bool SaveAll()
    {
      var rc = true;

      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {

          foreach (var dto in _dtos.Values)
            dto.Save();

          transaction.Commit();
          Directory.Delete(_extractPath, true);

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

}