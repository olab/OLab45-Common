using OLabWebAPI.Common;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace OLabWebAPI.Importer
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
    public readonly AppSettings Settings;
    private readonly IDictionary<DtoTypes, XmlDto> _dtos = new Dictionary<DtoTypes, XmlDto>();
    private readonly OLabLogger _logger;
    private readonly WikiTagProvider _tagProvider;

    public WikiTagProvider GetWikiProvider() { return _tagProvider; }
    public XmlDto GetDto(DtoTypes type) { return _dtos[type]; }
    public OLabDBContext GetContext() { return _context; }
    public OLabLogger GetLogger() { return _logger; }

    private string _logFileDirectory;
    private string _extractPath;
    private string _importFileName;

    public Importer(AppSettings settings, OLabLogger logger, OLabDBContext context)
    {
      Settings = settings;
      _context = context;
      this._logger = logger;

      _tagProvider = new WikiTagProvider(logger);

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
      _logger.LogInformation(message);
    }

    public void LogDebug(string message)
    {
      _logger.LogDebug(message);
    }

    public void LogWarning(string message)
    {
      _logger.LogWarning(message);
    }

    public void LogError(Exception ex, string message)
    {
      _logger.LogError(ex, message);
    }

    public void LogError(string message)
    {
      _logger.LogError(message);
    }

    private string Extract(string archiveFileName)
    {
      var tempDir = Path.GetTempPath();

      _logFileDirectory = Path.GetDirectoryName(archiveFileName);
      _extractPath = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(archiveFileName));
      _importFileName = Path.GetFileNameWithoutExtension(archiveFileName);

      if (Directory.Exists(_extractPath))
      {
        _logger.LogDebug($"Deleting existing extract directory");
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

        foreach (XmlDto dto in _dtos.Values)
          dto.Load(extractPath);

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $" load error: {ex.Message}");
      }

      return importStatus;
    }

    public bool SaveAll()
    {
      var rc = true;

      using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _context.Database.BeginTransaction())
      {
        try
        {
          _logger.LogInformation($"");
          _logger.LogInformation($"Saving data");

          foreach (XmlDto dto in _dtos.Values)
            dto.Save();

          transaction.Commit();
          Directory.Delete(_extractPath, true);

          var xmlMapDto = _dtos[DtoTypes.XmlMapDto] as XmlMapDto;
          var xmlMap = (XmlMap)xmlMapDto.GetDbPhys();

          _logger.LogInformation($" saved map '{xmlMap.Data[0].Name}'. id = {xmlMap.Data[0].Id}");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $" save error: {ex.Message}");
          transaction.Rollback();
        }

      }

      return rc;
    }

  }

}