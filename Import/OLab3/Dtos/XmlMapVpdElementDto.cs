using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapVpdElementDto : XmlImportDto<XmlMapVpdElements>
{
  private readonly Api.ObjectMapper.MapVpdElement _mapper;

  public XmlMapVpdElementDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapVpdElementDto,
      "map_vpd_element.xml")
  {
    _mapper = new Api.ObjectMapper.MapVpdElement(GetLogger(), GetDbContext(), GetWikiProvider());
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.map_vpd_element.Elements();
  }

  /// <summary>
  /// Saves import object to database
  /// </summary>
  /// <param name="dtos">All import dtos (for lookups into related objects)</param>
  /// <param name="elements">XML doc as an array of elements</param>
  /// <returns>Success/failure</returns>
  public override bool SaveToDatabase(
    string importFolderName,
    int recordIndex,
    IEnumerable<dynamic> elements)
  {
    var vpdElementPhys = _mapper.ElementsToPhys(elements);

    var vpdDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdDto) as XmlMapVpdDto;
    var vpdPhys = vpdDto.GetModel().Data.First(x => x.Id == vpdElementPhys.VpdId);

    // only support the VPDTextType type at this time
    if (vpdPhys.VpdTypeId != 1)
    {
      GetLogger().LogInformation($"  skipped vpd type {vpdPhys.VpdTypeId} {vpdElementPhys.Id} MapVpdElement record");
      return true;
    }

    var item = new SystemConstants();
    var oldId = vpdPhys.Id;

    item.Id = 0;
    item.ImageableType = "Maps";
    item.Name = vpdElementPhys.Key;
    item.Value = Encoding.ASCII.GetBytes(vpdElementPhys.Value);
    item.CreatedAt = DateTime.Now;
    item.Description = $"Imported from {GetFileName()} id = {vpdElementPhys.Id}.";

    var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
    item.ImageableId = mapDto.GetIdTranslation(GetFileName(), vpdPhys.MapId).Value;

    GetDbContext().SystemConstants.Add(item);
    GetDbContext().SaveChanges();

    // don't have a name, so save the id as the new name
    item.Name = item.Id.ToString();
    GetDbContext().SaveChanges();

    CreateIdTranslation(oldId, item.Id, Encoding.Default.GetString(item.Value));

    return true;
  }

  /// <summary>
  /// Add id translation record to store
  /// </summary>
  /// <param name="originalId">Import system Id</param>
  /// <param name="newId">Database id</param>
  protected bool CreateIdTranslation(uint originalId, uint? newId, string value)
  {
    if (_idTranslation.ContainsKey(originalId))
    {
      GetLogger().LogInformation($"  replaced {_fileName} = {value}. translation id {originalId} -> {newId.Value} ");
      _idTranslation[originalId] = newId;
      return false;
    }

    _idTranslation.Add(originalId, newId);
    GetLogger().LogInformation($"  added {_fileName} = {value}. translation id {originalId} -> {newId.Value} ");

    return true;
  }
}