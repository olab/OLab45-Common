using OLab.Api.Importer;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapVpdDto : XmlImportDto<XmlMapVpds>
{
  private readonly Api.ObjectMapper.MapVpd _mapper;

  public XmlMapVpdDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapVpdDto,
      "map_vpd.xml")
  {
    _mapper = new Api.ObjectMapper.MapVpd(logger);
  }

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="importDirectory">Directory where import file exists</param>
  /// <returns></returns>
  //public override async Task<bool> LoadAsync(
  //  string importFileDirectory, 
  //  bool displayProgressMessage = true)
  //{
  //  var rc = true;

  //  try
  //  {
  //    Logger.LogInformation($"Loading {GetFileName()}");

  //    using var memoryStream = await GetFileModule().ReadImportFileAsync(
  //        importFileDirectory,
  //        GetFileName());
  //    _phys = DynamicXml.Load(memoryStream);

  //    dynamic outerElements = GetElements(GetXmlPhys());
  //    var record = 0;

  //    foreach (var innerElements in outerElements)
  //    {
  //      try
  //      {
  //        ++record;
  //        var elements = (IEnumerable<dynamic>)innerElements.Elements();
  //        xmlImportElementSets.Add(elements);

  //        var item = _mapper.ElementsToPhys(elements);

  //        var phys = new XmlMapVpd
  //        {
  //          Id = item.Id,
  //          MapId = item.MapId,
  //          VpdTypeId = item.VpdTypeId
  //        };

  //        Logger.LogInformation($"  loaded '{phys.Id}'");

  //        GetModel().Data.Add(phys);
  //        record++;
  //      }
  //      catch (Exception ex)
  //      {
  //        Logger.LogError(ex, $"error loading '{GetFileName()}' record #{record}: {ex.Message}");
  //      }

  //    }

  //    Logger.LogInformation($"imported {xmlImportElementSets.Count()} {GetFileName()} objects");

  //    // delete data file
  //    await GetFileModule().DeleteImportFileAsync(
  //      importFileDirectory,
  //      GetFileName());
  //  }
  //  catch (Exception ex)
  //  {
  //    Logger.LogError(ex, $"Load error: {ex.Message}");
  //    rc = false;
  //  }

  //  return rc;
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
          var elements = (IEnumerable<dynamic>)innerElements.Elements();
          elementSets.Add(elements);

          var item = _mapper.ElementsToPhys(elements);

          var phys = new XmlMapVpd
          {
            Id = item.Id,
            MapId = item.MapId,
            VpdTypeId = item.VpdTypeId
          };

          Logger.LogInformation($"  loaded '{phys.Id}'");

          GetModel().Data.Add(phys);
          record++;
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, $"error loading '{GetFileName()}' record #{record}: {ex.Message}");
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
    return (IEnumerable<dynamic>)xmlPhys.map_vpd.Elements();
  }

  /// <summary>
  /// Saves import object to database
  /// </summary>
  /// <param name="dtos">All import dtos (for lookups into related objects)</param>
  /// <param name="elements">XML doc as an array of elements</param>
  /// <returns>Success/failure</returns>
  public override async Task<bool> SaveToDatabaseAsync(
    string importFolderName,
    int recordIndex,
    IEnumerable<dynamic> elements)
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