using System.Collections.Generic;

namespace OLab.Importer
{
  public class XmlMapNodeSectionNodeDto : XmlImportDto<XmlMapNodeSectionNode>
  {
    public XmlMapNodeSectionNodeDto(Importer importer) : base(importer, "map_node_section_node.xml")
    {

    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)GetXmlPhys().map_node_section_node.Elements();
    }
  }


}