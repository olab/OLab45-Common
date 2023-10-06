using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Import.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static OLab.Api.Importer.Importer;

namespace OLab.Api.Importer
{
  /// <summary>
  /// Xml import base object
  /// </summary>
  /// <typeparam name="P">Model (physical) class</typeparam>
  public abstract class XmlImportDto<P> : XmlDto where P : new()
  {
    protected dynamic _phys;
    private readonly string _fileName;
    protected readonly IOLabLogger Logger;
    private readonly IImporter _importer;
    protected OLabDBContext Context;
    private readonly string _websitePublicFilesDirectory;
    private string _importDirectory;

    private readonly IOLabModuleProvider<IWikiTagModule> _tagProvider;
    public IOLabModuleProvider<IWikiTagModule> GetWikiProvider() { return _tagProvider; }

    protected readonly P _modelObject = new P();

    public override bool PostProcess(IDictionary<Importer.DtoTypes, XmlDto> dtos) { return true; }
    public IImporter GetImporter() { return _importer; }
    public string GetFileName() { return _fileName; }
    public P GetModel() { return _modelObject; }
    public dynamic GetXmlPhys() { return _phys; }
    public override Object GetDbPhys() { return _modelObject; }
    public abstract IEnumerable<dynamic> GetElements(dynamic xmlPhys);
    public string GetImportPackageDirectory() { return _importDirectory; }
    public string GetWebsitePublicDirectory() { return _websitePublicFilesDirectory; }
    public void SetImportDirectory(string dir) { _importDirectory = dir; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="importer">Importer object</param>
    /// <param name="fileName">File name to import</param>
    public XmlImportDto(IOLabLogger logger, IImporter importer, DtoTypes dtoType, string fileName) : base( dtoType )
    {
      _importer = importer;
      _fileName = fileName;

      _tagProvider = GetImporter().GetWikiProvider();
      this.Logger = logger;
      Context = GetImporter().GetContext();
      _websitePublicFilesDirectory = GetImporter().Settings().FileStorageFolder;
    }

    /// <summary>
    /// Get postimport object creation id
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
    /// Get postimport object creation id
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
    /// <param name="importDirectory">Directory containing extracted import files</param>
    /// <returns>Success/Failure</returns>
    public override bool Load(string importDirectory)
    {
      var rc = true;

      try
      {
        _importDirectory = importDirectory;

        Logger.LogInformation($"Loading {GetFileName()}");

        if (_importer.GetFileStorageModule().FileExists(Logger, GetImportPackageDirectory(), GetFileName()))
        {
          var stream = _importer.GetFileStorageModule().ReadFileAsync(
            Logger, 
            importDirectory, 
            GetFileName()).GetAwaiter().GetResult();
          _phys = DynamicXml.Load(stream);
        }
        else
        {
          Logger.LogInformation($"File {GetFileName()} does not exist");
          return false;
        }

        dynamic outerElements = GetElements(GetXmlPhys());

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
    public override bool Save()
    {
      Logger.LogInformation($"Saving {xmlImportElementSets.Count()} {GetFileName()} objects");

      var recordIndex = 1;
      foreach (var elements in xmlImportElementSets)
      {
        try
        {
          Save(recordIndex, elements);
        }
        catch (System.Exception ex)
        {
          Logger.LogError($"Error {GetFileName()} record #{recordIndex}: {ex.Message}");
        }

        recordIndex++;
      }

      return true;
    }

    // implemented here so non-applicable derived classes
    // do not need to re-implement it
    public virtual bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Calculate target directory for scoped type and id
    /// </summary>
    /// <param name="parentType">Scoped object type (e.g. 'Maps')</param>
    /// <param name="parentId">Scoped object id</param>
    /// <param name="path">Optional file name</param>
    /// <returns>Public directory for scope</returns>
    public string GetPublicFileDirectory(string parentType, uint parentId, string path = "")
    {
      var targetDirectory = Path.Combine(GetWebsitePublicDirectory(), parentType);
      targetDirectory = Path.Combine(targetDirectory, parentId.ToString());

      if (!string.IsNullOrEmpty(path))
        targetDirectory = Path.Combine(targetDirectory, path);

      return targetDirectory;
    }

    /// <summary>
    /// Test if public file exists
    /// </summary>
    /// <param name="path">Full path to file</param>
    /// <returns>true/false</returns>
    public bool DoesPublicFileExist(string path)
    {
      var filePath = Path.Combine(GetWebsitePublicDirectory(), Path.GetFileName(path));
      return File.Exists(filePath);
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

}