using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapQuestionDto : XmlImportDto<XmlMapQuestions>
{
  private readonly Api.ObjectMapper.Questions _mapper;

  public XmlMapQuestionDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapQuestionDto, "map_question.xml" )
  {
    _mapper = new Api.ObjectMapper.Questions( logger, GetDbContext(), GetWikiProvider() );
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
  public override bool SaveToDatabase(
    string importFolderName,
    int recordIndex,
    IEnumerable<dynamic> elements)
  {
    var item = _mapper.ElementsToPhys( elements );
    var oldId = item.Id;

    item.Id = 0;

    var mapDto = GetImporter().GetDto( Importer.DtoTypes.XmlMapDto ) as XmlMapDto;
    item.ImageableId = mapDto.GetIdTranslation( GetFileName(), item.ImageableId ).Value;
    item.Description = $"Imported from {GetFileName()} id = {oldId}";

    var counterDto = GetImporter().GetDto( Importer.DtoTypes.XmlMapCounterDto ) as XmlMapCounterDto;
    if ( item.CounterId.HasValue )
      item.CounterId = counterDto.GetIdTranslation( GetFileName(), item.CounterId.Value );

    GetDbContext().SystemQuestions.Add( item );
    GetDbContext().SaveChanges();

    // don't have a name, so save the id as the new name
    item.Name = item.Id.ToString();
    GetDbContext().SaveChanges();

    CreateIdTranslation( oldId, item.Id );

    return true;
  }

}