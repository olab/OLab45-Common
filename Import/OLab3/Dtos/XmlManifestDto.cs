using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
  public override async Task<bool> LoadAsync(string extractPath, bool displayProgressMessage = true)
  {
    var result = await base.LoadAsync(extractPath);

    if (result)
    {
      dynamic elements = GetElements(GetXmlPhys());

      var record = 0;
      foreach (var element in elements)
      {
        try
        {
          ++record;
          dynamic value = Conversions.Base64Decode(element.Value) + ".xml";
          GetModel().Data.Add(value);
        }
        catch (Exception ex)
        {
          GetLogger().LogError(ex, $"Error loading '{GetFileName()}' record #{record}: {ex.Message}");
        }

      }
    }

    return result;
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