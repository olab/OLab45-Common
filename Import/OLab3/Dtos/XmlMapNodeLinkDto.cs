using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;

namespace OLab.Import.OLab3.Dtos
{

  public class XmlMapNodeLinkDto : XmlImportDto<XmlMapNodeLinks>
  {
    private readonly MapNodeLinksMapper _mapper;

    public XmlMapNodeLinkDto(
      IOLabLogger logger,
      Importer importer) : base(
        logger,
        importer,
        Importer.DtoTypes.XmlMapNodeLinkDto,
        "map_node_link.xml")
    {
      _mapper = new MapNodeLinksMapper(logger);
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_node_link.Elements();
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

      var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
      item.MapId = mapDto.GetIdTranslation(GetFileName(), item.MapId).Value;

      var nodeDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapNodeDto) as XmlMapNodeDto;
      item.NodeId1 = nodeDto.GetIdTranslation(GetFileName(), item.NodeId1).Value;
      item.NodeId2 = nodeDto.GetIdTranslation(GetFileName(), item.NodeId2).Value;

      Context.MapNodeLinks.Add(item);
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);

      return true;
    }

  }

}