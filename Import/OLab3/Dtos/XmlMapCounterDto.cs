using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapCounterDto : XmlImportDto<XmlMapCounters>
{
  private readonly Api.ObjectMapper.CounterMapper _mapper;

  public XmlMapCounterDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapCounterDto,
      "map_counter.xml")
  {
    _mapper = new Api.ObjectMapper.CounterMapper(logger);
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)GetXmlPhys().map_counter.Elements();
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
    var item = _mapper.ElementsToPhys(elements);
    item.ImageableId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "map_id").Value);

    var oldId = item.Id;

    item.Id = 0;

    var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
    item.ImageableId = mapDto.GetIdTranslation(GetFileName(), item.ImageableId).Value;
    item.ImageableType = "Maps";

    Context.SystemCounters.Add(item);
    Context.SaveChanges();

    CreateIdTranslation(oldId, item.Id);

    return true;
  }

}