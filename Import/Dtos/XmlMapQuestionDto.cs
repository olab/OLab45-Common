using System.Collections.Generic;

namespace OLab.Importer
{

  public class XmlMapQuestionDto : XmlImportDto<XmlMapQuestions>
  {
    private readonly ObjectMapper.Questions _mapper;

    public XmlMapQuestionDto(Importer importer) : base(importer, "map_question.xml")
    {
      _mapper = new ObjectMapper.Questions(GetLogger(), GetWikiProvider());
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_question.Elements();
    }

    /// <summary>
    /// Saves import object to database
    /// </summary>
    /// <param name="dtos">All import dtos (for lookups into related objects)</param>
    /// <param name="elements">XML doc as an array of elements</param>
    /// <returns>Success/failure</returns>
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      Model.SystemQuestions item = _mapper.ElementsToPhys(elements);
      var oldId = item.Id;

      item.Id = 0;

      var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
      item.ImageableId = mapDto.GetIdTranslation(GetFileName(), item.ImageableId).Value;
      item.Description = $"Imported from {GetFileName()} id = {oldId}";

      var counterDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapCounterDto) as XmlMapCounterDto;
      if (item.CounterId.HasValue)
        item.CounterId = counterDto.GetIdTranslation(GetFileName(), item.CounterId.Value);

      Context.SystemQuestions.Add(item);
      Context.SaveChanges();

      // don't have a name, so save the id as the new name
      item.Name = item.Id.ToString();
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);
      GetLogger().LogDebug($"Saved {GetFileName()} id {oldId} -> {item.Id}");

      return true;
    }

  }

}