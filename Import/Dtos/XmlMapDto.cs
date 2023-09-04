using OLabWebAPI.ObjectMapper;
using System.Collections.Generic;

namespace OLabWebAPI.Importer
{

  public class XmlMapDto : XmlImportDto<XmlMap>
  {
    private readonly MapsMapper _mapper;

    public XmlMapDto(Importer importer) : base(importer, "map.xml")
    {
      _mapper = new MapsMapper(GetLogger(), GetWikiProvider());
    }

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
      Model.Maps item = _mapper.ElementsToPhys(elements);
      var oldId = item.Id;
      item.Id = 0;

      Context.Maps.Add(item);
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);
      GetLogger().LogDebug($" saved {GetFileName()} id {oldId} -> {item.Id}");

      GetModel().Data.Add(item);

      return true;
    }

  }
}