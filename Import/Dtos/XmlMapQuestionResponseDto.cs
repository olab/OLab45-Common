using OLab.Common.Interfaces;
using System.Collections.Generic;

namespace OLab.Api.Importer
{

  public class XmlMapQuestionResponseDto : XmlImportDto<XmlMapQuestionResponses>
  {
    private readonly ObjectMapper.QuestionResponses _mapper;

    public XmlMapQuestionResponseDto(
      IOLabLogger logger,
      Importer importer) : base(
        logger,
        importer,
        Importer.DtoTypes.XmlMapQuestionResponseDto,
        "map_question_response.xml")
    {
      _mapper = new ObjectMapper.QuestionResponses(logger, GetWikiProvider());
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_question_response.Elements();
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

      var questionDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapQuestionDto) as XmlMapQuestionDto;
      item.QuestionId = questionDto.GetIdTranslation(GetFileName(), item.QuestionId.Value);
      item.Description = $" saved {GetFileName()} id = {oldId}";

      Context.SystemQuestionResponses.Add(item);
      Context.SaveChanges();

      // don't have a name, so save the id as the new name
      item.Name = item.Id.ToString();
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);

      return true;
    }
  }

}