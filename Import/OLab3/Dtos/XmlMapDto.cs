using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapDto : XmlImportDto<XmlMap>
{
  private readonly MapsMapper _mapper;

  public XmlMapDto(IOLabLogger logger, Importer importer) : base(logger, importer, Importer.DtoTypes.XmlMapDto, "map.xml")
  {
    _mapper = new MapsMapper(logger);
  }

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="importDirectory">Directory where import file exists</param>
  /// <returns></returns>
  // public override bool Load(Stream stream)
  // {
  //   var result = base.Load(importDirectory);
  //   var elements = GetElements(GetXmlPhys());
  //   xmlImportElementSets.Add(elements);

  //   return result;
  // }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.Elements();
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

    Context.Maps.Add(item);
    Context.SaveChanges();

    CreateIdTranslation(oldId, item.Id);
    GetModel().Data.Add(item);

    return true;
  }

}