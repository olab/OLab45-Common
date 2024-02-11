using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;

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

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="importDirectory">Directory where import file exists</param>
  /// <returns></returns>
  public override bool Load(string extractPath)
  {
    var result = base.Load(extractPath);

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
          Logger.LogError(ex, $"Error loading '{GetFileName()}' record #{record}: {ex.Message}");
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

  // there is no Save for Manifest records
  public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
  {
    return true;
  }

}