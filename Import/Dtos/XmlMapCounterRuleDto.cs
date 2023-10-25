using OLab.Common.Interfaces;
using System.Collections.Generic;

namespace OLab.Api.Importer
{

  public class XmlMapCounterRuleDto : XmlImportDto<XmlMapCounterRule>
  {
    public XmlMapCounterRuleDto(
      IOLabLogger logger,
      Importer importer) : base(
        logger,
        importer,
        Importer.DtoTypes.XmlMapCounterRuleDto,
        "map_counter_rule.xml")
    {

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

    // there is no applicable implementation for this yet
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      return true;
    }
  }

}