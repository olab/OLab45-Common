using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;

namespace OLabWebAPI.Importer
{
  /// <summary>
  /// Xml import Manifest object DTO
  /// </summary>
  public class XmlManifestDto : XmlImportDto<XmlManifest>
  {
    public XmlManifestDto(Importer importer) : base(importer, "manifest.xml") { }

    /// <summary>
    /// Loads the specific import file into a model object
    /// </summary>
    /// <param name="importDirectory">Directory where import file exists</param>
    /// <returns></returns>
    public override bool Load(string importDirectory)
    {
      var result = base.Load(importDirectory);

      if (result)
      {
        dynamic elements = GetElements(GetXmlPhys());

        var record = 0;
        foreach (dynamic element in elements)
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

    // there is no Save for Manifest records
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      return true;
    }

  }


}