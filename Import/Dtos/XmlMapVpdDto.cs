using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OLab.Api.Importer
{

  public class XmlMapVpdDto : XmlImportDto<XmlMapVpds>
  {
    private readonly ObjectMapper.MapVpd _mapper;

    public XmlMapVpdDto(
      IOLabLogger logger, 
      Importer importer) : base(
        logger, 
        importer, 
        Importer.DtoTypes.XmlMapVpdDto, 
        "map_vpd.xml")
    {
      _mapper = new ObjectMapper.MapVpd(logger);
    }

    /// <summary>
    /// Loads the specific import file into a model object
    /// </summary>
    /// <param name="importDirectory">Directory where import file exists</param>
    /// <returns></returns>
    public override bool Load(string importDirectory)
    {
      var rc = true;

      try
      {
        SetImportDirectory(importDirectory);

        var filePath = $"{GetImportFilesDirectory()}{GetFileStorageModule().GetFolderSeparator()}{GetFileName()}";
        Logger.LogInformation($"Loading '{filePath}'");

        if (GetFileStorageModule().FileExists(Logger, GetImportFilesDirectory(), GetFileName()))
        {
          var stream = GetFileStorageModule().ReadFileAsync(
            Logger, 
            GetImportFilesDirectory(), 
            GetFileName()).GetAwaiter().GetResult();

          _phys = DynamicXml.Load(stream);
        }
        else
        {
          Logger.LogInformation($"File {filePath} does not exist");
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

            var item = _mapper.ElementsToPhys(elements);

            var phys = new XmlMapVpd
            {
              Id = item.Id,
              MapId = item.MapId,
              VpdTypeId = item.VpdTypeId
            };

            GetModel().Data.Add(phys);
            record++;
          }
          catch (Exception ex)
          {
            Logger.LogError(ex, $"error loading '{GetFileName()}' record #{record}: {ex.Message}");
          }

        }

        Logger.LogInformation($"imported {xmlImportElementSets.Count()} {GetFileName()} objects");

      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Load error: {ex.Message}");
        rc = false;
      }

      return rc;
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_vpd.Elements();
    }

    /// <summary>
    /// Saves import object to database
    /// </summary>
    /// <param name="dtos">All import dtos (for lookups into related objects)</param>
    /// <param name="elements">XML doc as an array of elements</param>
    /// <returns>Success/failure</returns>
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      var item = _mapper.ElementsToPhys(elements);
      var oldId = item.Id;
      item.Id = 0;

      Context.MapVpds.Add(item);
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);

      return true;
    }

  }
}