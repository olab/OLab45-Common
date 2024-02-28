using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapNodeDto : XmlImportDto<XmlMapNodes>
{
  private readonly Api.ObjectMapper.MapNodesMapper _mapper;
  private readonly IOLabConfiguration _configuration;

  public XmlMapNodeDto(IOLabLogger logger, Importer importer) : base(logger, importer, Importer.DtoTypes.XmlMapNodeDto, "map_node.xml")
  {
    _mapper = new Api.ObjectMapper.MapNodesMapper(logger);
    _configuration = importer.GetConfiguration();
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.map_node.Elements();
  }

  /// <summary>
  /// Replaces (deprecated) VPD tags with CONST
  /// </summary>
  /// <param name="item">Source MapNode</param>
  /// <returns>true if found replacement</returns>
  public bool ReplaceVpdWikiTags(MapNodes item)
  {
    var rc = false;

    // replace all VPD with CONST in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdElementDto) as XmlMapVpdElementDto;

    var wiki = new VpdWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt32(wiki.GetWikiArgument1());
      var wikiTag = wiki.GetWikiType();

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wikiTag.Replace(id.ToString(), newId.ToString());
      newWikiTag = newWikiTag.Replace(wiki.GetWikiType(), "CONST");

      Logger.LogInformation($"Replacing '{wikiTag}' -> '{newWikiTag}'");
      item.Text = item.Text.Replace(wikiTag, newWikiTag);

      rc = true;
    }

    return rc;
  }

  /// <summary>
  /// Replaces (deprecated) Avatar tags with CONST
  /// </summary>
  /// <param name="item">Source MapNode</param>
  /// <returns>true if found replacement</returns>
  public bool ReplaceAvWikiTags(MapNodes item)
  {
    var rc = false;

    // replace all VPD with CONST in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapAvatarDto) as XmlMapAvatarDto;

    var wiki = new AvatarWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt16(wiki.GetWikiArgument1());
      var wikiTag = wiki.GetWikiType();

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wikiTag.Replace(id.ToString(), newId.ToString());
      newWikiTag = newWikiTag.Replace(wiki.GetWikiType(), "MR");

      Logger.LogInformation($"Replacing '{wikiTag}' -> '{newWikiTag}'");
      item.Text = item.Text.Replace(wikiTag, newWikiTag);

      rc = true;
    }

    return rc;
  }

  public bool RemapMrWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all MR with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapElementDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new MediaResourceWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
      var tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

      item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

      mappedWikiTags.Add(tmpWikiTag, newWikiTag);
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      Logger.LogInformation($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }

  public bool RemapQuWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all QU with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapQuestionDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new QuestionWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
      var tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

      item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

      mappedWikiTags.Add(tmpWikiTag, newWikiTag);
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      Logger.LogInformation($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }

  public bool ReplaceCrWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all QU with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapCounterDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new CounterWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {

      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
      var tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

      item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

      mappedWikiTags.Add(tmpWikiTag, newWikiTag);
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      Logger.LogInformation($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }

  public bool ReplaceConstWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all QU with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdElementDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new ConstantWikiTag(Logger, _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {

      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
      var tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

      item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

      mappedWikiTags.Add(tmpWikiTag, newWikiTag);
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      Logger.LogInformation($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
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
    var item = _mapper.ElementsToPhys(elements);
    var oldId = item.Id;

    CurrentRecordIndex = recordIndex;

    item.Id = 0;

    var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto);
    item.MapId = mapDto.GetIdTranslation(GetFileName(), item.MapId).Value;

    // remap 'true' MR's before Avatars since Avatars are rendered as MR's.
    RemapMrWikiTags(item);
    RemapQuWikiTags(item);
    ReplaceVpdWikiTags(item);
    ReplaceAvWikiTags(item);
    ReplaceConstWikiTags(item);
    ReplaceCrWikiTags(item);

    item.Info = $"\nImported from map_node.xml. id = {oldId}";

    Context.MapNodes.Add(item);
    Context.SaveChanges();

    CreateIdTranslation(oldId, item.Id);
    GetModel().Data.Add(item);

    return true;
  }

}