using OLabWebAPI.Dto;
using System;
using System.Collections.Generic;

namespace OLabWebAPI.Importer
{

  public class XmlMapNodeDto : XmlImportDto<XmlMapNodes>
  {
    private readonly ObjectMapper.MapNodesMapper _mapper;

    public XmlMapNodeDto(Importer importer) : base(importer, "map_node.xml")
    {
      _mapper = new ObjectMapper.MapNodesMapper(GetLogger(), GetWikiProvider());
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
    public bool ReplaceVpdWikiTags(Model.MapNodes item)
    {
      bool rc = false;

      // replace all VPD with CONST in node text
      XmlMapVpdDto vpdDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdElementDto) as XmlMapVpdDto;

      VpdWikiTag wiki = new VpdWikiTag(GetLogger());
      while (wiki.HaveWikiTag(item.Text))
      {
        uint id = Convert.ToUInt32(wiki.GetWikiArgument1());
        string wikiTag = wiki.GetWikiType();

        uint? newId = vpdDto.GetIdTranslation(GetFileName(), id);

        string newWikiTag = wikiTag.Replace(id.ToString(), newId.ToString());
        newWikiTag = newWikiTag.Replace(wiki.GetWikiType(), "CONST");

        GetLogger().LogDebug($"Replacing '{wikiTag}' -> '{newWikiTag}'");
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
    public bool ReplaceAvWikiTags(Model.MapNodes item)
    {
      bool rc = false;

      // replace all VPD with CONST in node text
      XmlMapAvatarDto avDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapAvatarDto) as XmlMapAvatarDto;

      AvatarWikiTag wiki = new AvatarWikiTag(GetLogger());
      while (wiki.HaveWikiTag(item.Text))
      {
        ushort id = Convert.ToUInt16(wiki.GetWikiArgument1());
        string wikiTag = wiki.GetWikiType();

        int? newId = avDto.GetIdTranslation(GetFileName(), id);

        string newWikiTag = wikiTag.Replace(id.ToString(), newId.ToString());
        newWikiTag = newWikiTag.Replace(wiki.GetWikiType(), "MR");

        GetLogger().LogDebug($"Replacing '{wikiTag}' -> '{newWikiTag}'");
        item.Text = item.Text.Replace(wikiTag, newWikiTag);

        rc = true;
      }

      return rc;
    }

    public bool RemapMrWikiTags(Model.MapNodes item)
    {
      bool rc = true;

      // remap all MR with new id's in node text
      XmlDto mapElementDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapElementDto);
      Dictionary<string, string> mappedWikiTags = new Dictionary<string, string>();

      MediaResourceWikiTag wiki = new MediaResourceWikiTag(GetLogger());
      while (wiki.HaveWikiTag(item.Text))
      {
        uint id = Convert.ToUInt32(wiki.GetWikiArgument1());

        uint? newId = mapElementDto.GetIdTranslation(GetFileName(), id);

        string newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
        string tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

        item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

        mappedWikiTags.Add(tmpWikiTag, newWikiTag);
      }

      foreach (string key in mappedWikiTags.Keys)
      {
        GetLogger().LogDebug($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
        item.Text = item.Text.Replace(key, mappedWikiTags[key]);
      }

      return rc;
    }

    public bool RemapQrWikiTags(Model.MapNodes item)
    {
      bool rc = true;

      // remap all MR with new id's in node text
      XmlDto questionDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapQuestionDto);
      Dictionary<string, string> mappedWikiTags = new Dictionary<string, string>();

      QuestionWikiTag wiki = new QuestionWikiTag(GetLogger());
      while (wiki.HaveWikiTag(item.Text))
      {
        uint id = Convert.ToUInt32(wiki.GetWikiArgument1());

        uint? newId = questionDto.GetIdTranslation(GetFileName(), id);

        string newWikiTag = wiki.PreviewNewArgument1(newId.ToString());
        string tmpWikiTag = wiki.GetWiki().Replace(wiki.GetWikiType(), "temp");

        item.Text = item.Text.Replace(wiki.GetWiki(), tmpWikiTag);

        mappedWikiTags.Add(tmpWikiTag, newWikiTag);
      }

      foreach (string key in mappedWikiTags.Keys)
      {
        GetLogger().LogDebug($"Remapping '{key}' -> '{mappedWikiTags[key]}'");
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
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      Model.MapNodes item = _mapper.ElementsToPhys(elements);
      uint oldId = item.Id;

      item.Id = 0;

      XmlDto mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto);
      item.MapId = mapDto.GetIdTranslation(GetFileName(), item.MapId).Value;

      // remap 'true' MR's before Avatars since Avatars are rendered as MR's.
      RemapMrWikiTags(item);
      RemapQrWikiTags(item);
      ReplaceVpdWikiTags(item);
      ReplaceAvWikiTags(item);

      item.Info = $"\nImported from map_node.xml. id = {oldId}";

      Context.MapNodes.Add(item);
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);

      GetModel().Data.Add(item);

      GetLogger().LogDebug($"Saved {GetFileName()} id {oldId} -> {item.Id}");

      return true;
    }

  }

}