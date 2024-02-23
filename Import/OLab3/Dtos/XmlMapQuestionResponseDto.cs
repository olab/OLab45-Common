using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapQuestionResponseDto : XmlImportDto<XmlMapQuestionResponses>
{
  private readonly Api.ObjectMapper.QuestionResponses _mapper;

  public XmlMapQuestionResponseDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapQuestionResponseDto,
      "map_question_response.xml")
  {
    _mapper = new Api.ObjectMapper.QuestionResponses(logger, GetWikiProvider());
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
  public override bool Save(
    string importFolderName, 
    int recordIndex, 
    IEnumerable<dynamic> elements)
  {
    var item = _mapper.ElementsToPhys(elements);
    var oldId = item.Id;
    var oldQuestionId = item.QuestionId.Value;

    item.Id = 0;

    var questionDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapQuestionDto) as XmlMapQuestionDto;
    item.QuestionId = questionDto.GetIdTranslation(GetFileName(), item.QuestionId.Value);
    item.Description = $"Imported from {GetFileName()} id = {oldId}";

    Context.SystemQuestionResponses.Add(item);
    Context.SaveChanges();

    // don't have a name, so save the id as the new name
    item.Name = item.Id.ToString();
    Context.SaveChanges();

    if ( CreateIdTranslation(oldId, item.Id) )
      Logger.LogInformation($"  added {_fileName} translation {oldId} -> {item.Id}, question {oldQuestionId} -> {item.QuestionId}");

    return true;
  }

  /// <summary>
  /// Add id translation record to store
  /// </summary>
  /// <param name="originalId">Import system Id</param>
  /// <param name="newId">Database id</param>
  protected override bool CreateIdTranslation(uint originalId, uint? newId = null)
  {
    if (_idTranslation.ContainsKey(originalId))
    {
      Logger.LogInformation($"  replaced {_fileName} translation {originalId} -> {newId.Value}");
      _idTranslation[originalId] = newId;
      return false;
    }

    _idTranslation.Add(originalId, newId);
    return true;
  }
}