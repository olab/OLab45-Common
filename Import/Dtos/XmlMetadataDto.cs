using OLab.Common.Interfaces;
using System.Collections.Generic;

namespace OLab.Api.Importer
{

  public class XmlMetadataDto : XmlImportDto<XmlMetadata>
  {
    public XmlMetadataDto(
      IOLabLogger logger,
      Importer importer) : base(
        logger,
        importer,
        Importer.DtoTypes.XmlMetadataDto,
        "metadata.xml")
    {

    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.metadata.Elements();
    }

    // there is no Save for MetaData records
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      return true;
    }
  }
}