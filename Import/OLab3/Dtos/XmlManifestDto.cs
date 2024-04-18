using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OLab.Import.OLab3.Dtos;

/// <summary>
/// Xml import Manifest object DTO
/// </summary>
public class XmlManifestDto : XmlImportDto<XmlManifest>
{
  public XmlManifestDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlManifestDto,
      "manifest.xml")
  { }

  public override string GetLoggerString(IEnumerable<dynamic> elements)
  {
    return $"";
  }

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="importDirectory">Directory where import file exists</param>
  /// <returns></returns>
  //public override async Task<bool> LoadAsync(string extractPath, bool displayProgressMessage = true)
  //{
  //  var result = await base.LoadAsync(extractPath);

  //  if (result)
  //  {
  //    dynamic elements = GetElements(GetXmlPhys());

  //    var record = 0;
  //    foreach (var element in elements)
  //    {
  //      try
  //      {
  //        ++record;
  //        dynamic value = Conversions.Base64Decode(element.Value) + ".xml";
  //        GetModel().Data.Add(value);
  //      }
  //      catch (Exception ex)
  //      {
  //        Logger.LogError(ex, $"Error loading '{GetFileName()}' record #{record}: {ex.Message}");
  //      }

  //    }
  //  }

  //  return result;
  //}


  protected override IList<IEnumerable<dynamic>> GetXmlElements(
    bool displayProgressMessage,
    dynamic outerElements)
  {
    var record = 0;
    var elementSets = new List<IEnumerable<dynamic>>();

    if (outerElements != null)
    {
      foreach (var innerElements in outerElements)
      {
        try
        {
          ++record;
          dynamic value = Conversions.Base64Decode(innerElements.Value) + ".xml";
          GetModel().Data.Add(value);
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, $"Error loading '{GetFileName()}' record #{record}: {ex.Message}");
        }
      }

      Logger.LogInformation($"imported {elementSets.Count()} {GetFileName()} objects");
    }

    return elementSets;
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.manifest.manifest_files.Elements();
  }

}