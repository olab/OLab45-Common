using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OLab.Import.OLab3.Dtos;

public class XmlMapNodeSectionDto : XmlImportDto<XmlMapNodeSection>
{
  public XmlMapNodeSectionDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapNodeSectionDto,
      "map_node_section.xml")
  {

  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return null;
  }

}