using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapCounterRuleDto : XmlImportDto<XmlMapCounterRule>
{
  private readonly Api.ObjectMapper.CounterActionsMapper _mapper;

  public XmlMapCounterRuleDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapCounterRuleDto,
      "map_counter_rule.xml")
  {
    _mapper = new Api.ObjectMapper.CounterActionsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider());
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)GetXmlPhys().map_counter_rule.Elements();
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
    item.CounterId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "counter_id").Value);

    var oldId = item.Id;

    item.Id = 0;

    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapCounterDto);
    item.CounterId = dto.GetIdTranslation(GetFileName(), item.CounterId).Value;

    GetDbContext().SystemCounterActions.Add(item);
    GetDbContext().SaveChanges();

    CreateIdTranslation(oldId, item.Id);

    return true;
  }


}