using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OLab.Import.OLab3.Importer;

namespace OLab.Import.OLab3.Dtos;

/// <summary>
/// Xml import base object
/// </summary>
/// <typeparam name="P">Model (physical) class</typeparam>
public abstract class XmlImportDto<P> : XmlDto where P : new()
{
  protected dynamic _phys;
  protected readonly string _fileName;
  protected readonly Importer _importer;
  protected OLabDBContext Context;
  protected int CurrentRecordIndex = 0;

  private readonly IOLabModuleProvider<IWikiTagModule> _tagProvider;
  public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _tagProvider; }

  protected readonly P _modelObject = new P();

  public override bool PostProcess(IDictionary<DtoTypes, XmlDto> dtos) { return true; }
  public Importer GetImporter() { return _importer; }
  public string GetFileName() { return _fileName; }
  public P GetModel() { return _modelObject; }
  public dynamic GetXmlPhys() { return _phys; }
  public override object GetDbPhys() { return _modelObject; }
  public abstract IEnumerable<dynamic> GetElements(dynamic xmlPhys);

  public IFileStorageModule GetFileModule()
  {
    return GetImporter().GetFileStorageModule();
  }

  /// <summary>
  /// Default constructor
  /// </summary>
  /// <param name="importer">Importer object</param>
  /// <param name="fileName">File name to import</param>
  public XmlImportDto(
    IOLabLogger logger, 
    Importer importer, 
    DtoTypes dtoType, 
    string fileName) : base(logger, dtoType)
  {
    _importer = importer;
    _fileName = fileName;

    _tagProvider = GetImporter().GetWikiProvider();
    Context = GetImporter().GetDbContext();
  }

  /// <summary>
  /// Add id translation record to store
  /// </summary>
  /// <param name="originalId">Import system Id</param>
  /// <param name="newId">Database id</param>
  protected override bool CreateIdTranslation(uint originalId, uint? newId = null)
  {
    if (_idTranslation.ContainsKey(originalId))
    {
      Logger.LogInformation($"  replaced {_fileName} translation {originalId} -> {newId.Value}");
      _idTranslation[originalId] = newId;
      return false;
    }

    _idTranslation.Add(originalId, newId);
    Logger.LogInformation($"  added {_fileName} translation {originalId} -> {newId.Value}");

    return true;
  }

  /// <summary>
  /// ReadAsync postimport object creation id
  /// </summary>
  /// <param name="originalId">Import system id</param>
  /// <returns></returns>
  public override uint? GetIdTranslation(string referencedFile, uint originalId)
  {
    if (originalId == 0)
      return 0;

    if (_idTranslation.TryGetValue(originalId, out var newId))
      return newId;

    throw new KeyNotFoundException($"references {GetFileName()} Id {originalId}: not found");
  }

  /// <summary>
  /// ReadAsync postimport object creation id
  /// </summary>
  /// <param name="originalId">(nullable) original id</param>
  /// <returns></returns>
  public override int? GetIdTranslation(string referencedFile, int? originalId)
  {
    if (!originalId.HasValue)
      return originalId;
    var value = GetIdTranslation(referencedFile, (uint)originalId.Value);
    return (int?)value;
  }

  /// <summary>
  /// Load import files
  /// </summary>
  /// <param name="importFilesFolder">Folder name of extracted import files</param>
  /// <returns>Success/Failure</returns>
  public override async Task<bool> LoadAsync(string importFileDirectory)
  {
    var rc = true;

    try
    {

      var moduleFileName = $"{importFileDirectory}{GetFileModule().GetFolderSeparator()}{GetFileName()}";
      Logger.LogInformation($"Loading {moduleFileName}");

      if (GetFileModule().FileExists(importFileDirectory, GetFileName()))
      {
        using var moduleFileStream = new MemoryStream();
        await GetFileModule().ReadFileAsync(
          moduleFileStream, 
          importFileDirectory, 
          GetFileName(), 
          new System.Threading.CancellationToken());
        _phys = DynamicXml.Load(moduleFileStream);
      }
      else
      {
        Logger.LogInformation($"File {importFileDirectory}{GetFileModule().GetFolderSeparator()}{GetFileName()} does not exist");
        return false;
      }

      dynamic outerElements = GetElements(GetXmlPhys());

      if (outerElements != null)
      {
        var record = 0;

        foreach (var innerElements in outerElements)
        {
          try
          {
            ++record;
            var elements = (IEnumerable<dynamic>)innerElements.Elements();
            xmlImportElementSets.Add(elements);
          }
          catch (Exception ex)
          {
            Logger.LogError(ex, $"Error loading '{GetFileName()}' record #{record}: {ex.Message}");
          }
        }

        Logger.LogInformation($"imported {xmlImportElementSets.Count()} {GetFileName()} objects");
      }

      // delete data file
      await GetFileModule().DeleteFileAsync(importFileDirectory, GetFileName());

    }
    catch (Exception ex)
    {
      if (!ex.Message.Contains("File not found"))
        Logger.LogError(ex, $"load error: {ex.Message}");
      rc = false;
    }

    return rc;
  }

  /// <summary>
  /// Save XmlDto's to physical database
  /// </summary>
  /// <param name="dtos">All import dtos</param>
  /// <returns>Success/Failure</returns>
  public override bool SaveToDatabase(string importFolderName)
  {
    Logger.LogInformation($"Saving {xmlImportElementSets.Count()} {GetFileName()} objects");

    var recordIndex = 1;
    foreach (var elements in xmlImportElementSets)
    {
      try
      {
        SaveToDatabase(importFolderName, recordIndex, elements);
      }
      catch (Exception ex)
      {
        Logger.LogError($"Error {GetFileName()} record #{recordIndex}: {ex.Message}");
      }

      recordIndex++;
    }

    return true;
  }

  // implemented here so non-applicable derived classes
  // do not need to re-implement it
  public virtual bool SaveToDatabase(string importFolderName, int recordIndex, IEnumerable<dynamic> elements)
  {
    return true;
  }

  public static IList<string> GetWikiTags(string source)
  {
    var tags = new List<string>();
    return tags;
  }

  public static bool ReplaceWikiTags(string haystack, string needle, string newNeedle)
  {
    return true;
  }


}