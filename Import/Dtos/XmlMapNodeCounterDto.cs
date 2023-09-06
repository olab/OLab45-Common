using OLab.Api.ObjectMapper;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.Importer
{

  public class XmlMapNodeCounterDto : XmlImportDto<XmlMapNodeCounters>
  {
    private readonly CounterActionsMapper _mapper;

    public XmlMapNodeCounterDto(Importer importer) : base(importer, "map_node_counter.xml")
    {
      _mapper = new CounterActionsMapper(GetLogger(), GetWikiProvider());
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_node_counter.Elements();
    }

    /// <summary>
    /// Saves import object to database
    /// </summary>
    /// <param name="dtos">All import dtos (for lookups into related objects)</param>
    /// <param name="elements">XML doc as an array of elements</param>
    /// <returns>Success/failure</returns>
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      Model.SystemCounterActions item = _mapper.ElementsToPhys(elements);
      var oldId = item.Id;

      // test for empty value/expression.  if so, igmore this save
      if (string.IsNullOrEmpty(item.Expression))
      {
        GetLogger().LogDebug($"Empty {GetFileName()} id = {oldId} value.  Skipping");
        return true;
      }

      item.Id = 0;

      var nodeDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapNodeDto) as XmlImportDto<XmlMapNodes>;
      item.ImageableId = nodeDto.GetIdTranslation(GetFileName(), item.ImageableId).Value;

      var counterDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapCounterDto) as XmlMapCounterDto;
      item.CounterId = counterDto.GetIdTranslation(GetFileName(), item.CounterId).Value;

      XmlDto mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto);
      Model.MapNodes node = nodeDto.GetModel().Data.Where(x => x.Id == item.ImageableId).FirstOrDefault();
      item.MapId = node.MapId;

      Context.SystemCounterActions.Add(item);
      Context.SaveChanges();

      GetLogger().LogDebug($"Saved {GetFileName()} id {oldId} -> {item.Id}");
      CreateIdTranslation(oldId, item.Id);

      return true;
    }

  }

}