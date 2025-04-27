using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapNodeDto : XmlImportDto<XmlMapNodes>
{
  private readonly Api.ObjectMapper.MapNodesMapper _mapper;
  private readonly IOLabConfiguration _configuration;

  public XmlMapNodeDto(IOLabLogger logger, Importer importer) : base( logger, importer, Importer.DtoTypes.XmlMapNodeDto, "map_node.xml" )
  {
    _mapper = new Api.ObjectMapper.MapNodesMapper( GetLogger(), GetDbContext(), GetWikiProvider() );
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
    var physMapNode = _mapper.ElementsToPhys( elements );
    var oldId = physMapNode.Id;

    CurrentRecordIndex = recordIndex;

    physMapNode.Id = 0;

    var mapDto = GetImporter().GetDto( Importer.DtoTypes.XmlMapDto );
    physMapNode.MapId = mapDto.GetIdTranslation( GetFileName(), physMapNode.MapId ).Value;

    // remap 'true' MR's before Avatars since Avatars are rendered as MR's.
    RemapWikiTags<MediaResourceWikiTag>( physMapNode, Importer.DtoTypes.XmlMapElementDto );
    RemapWikiTags<QuestionWikiTag>( physMapNode, Importer.DtoTypes.XmlMapQuestionDto );
    RemapWikiTags<ConstantWikiTag>( physMapNode, Importer.DtoTypes.XmlMapVpdElementDto );
    RemapWikiTags<CounterWikiTag>( physMapNode, Importer.DtoTypes.XmlMapCounterDto );
    ReplaceVpdWikiTags( physMapNode );
    ReplaceAvWikiTags( physMapNode );

    physMapNode.Info = $"\nImported from map_node.xml. id = {oldId}";

    GetDbContext().MapNodes.Add( physMapNode );
    GetDbContext().SaveChanges();

    CreateIdTranslation( oldId, physMapNode.Id );
    GetModel().Data.Add( physMapNode );

    return true;
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
    var dto = GetImporter().GetDto( Importer.DtoTypes.XmlMapVpdElementDto ) as XmlMapVpdElementDto;

    var wiki = new VpdWikiTag( GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( item.Text ) )
    {
      try
      {
        var id = Convert.ToUInt32( wiki.GetWikiArgument1() );
        var newId = dto.GetIdTranslation( GetFileName(), id );

        var newWiki = new VpdWikiTag( GetLogger(), _configuration );
        newWiki.Set( "CONST", newId.Value.ToString() );

        GetLogger().LogInformation( $"    replacing '{wiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
        item.Text = item.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

        rc = true;
      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: MapNode '{item.Title}': could not resolve: '{wiki.GetWiki()}'" );

        item.Text = item.Text.Replace( wiki.GetWiki(), $"{wiki.GetUnquotedWiki()}: could not resolve" );

        rc = false;
      }
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
    var dto = GetImporter().GetDto( Importer.DtoTypes.XmlMapAvatarDto ) as XmlMapAvatarDto;

    var wiki = new AvatarWikiTag( GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( item.Text ) )
    {
      var id = Convert.ToUInt16( wiki.GetWikiArgument1() );
      var newId = dto.GetIdTranslation( GetFileName(), id );

      var newWiki = new AvatarWikiTag( GetLogger(), _configuration );
      newWiki.Set( "MR", newId.Value.ToString() );

      GetLogger().LogInformation( $"    replacing '{wiki.GetWiki()}' -> '{newWiki.GetWiki()}'" );
      item.Text = item.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

      rc = true;
    }

    return rc;
  }

  public bool RemapWikiTags<T>(MapNodes item, Importer.DtoTypes dtoType) where T : WikiTag1ArgumentModule
  {
    var rc = true;

    // remap all MR with new id's in node text
    var dto = GetImporter().GetDto( dtoType );
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );
    while ( wiki.HaveWikiTag( item.Text ) )
    {
      var id = Convert.ToUInt32( wiki.GetWikiArgument1() );

      try
      {
        var newId = dto.GetIdTranslation( GetFileName(), id );

        var newWiki = (T)Activator.CreateInstance( typeof( T ), GetLogger(), _configuration );
        newWiki.Set( wiki.GetWikiType().ToLower(), newId.Value.ToString() );

        item.Text = item.Text.Replace( wiki.GetWiki(), newWiki.GetWiki() );

        mappedWikiTags.Add( newWiki.GetWiki(), wiki.GetWiki() );
      }
      catch ( KeyNotFoundException )
      {
        GetLogger().LogError( $"ERROR: MapNode '{item.Title}': could not resolve: '{wiki.GetWiki()}'" );

        item.Text = item.Text.Replace( wiki.GetWiki(), $"{wiki.GetUnquotedWiki()}: could not resolve" );

        rc = false;
      }

    }

    foreach ( var key in mappedWikiTags.Keys )
    {
      GetLogger().LogInformation( $"    remapping '{mappedWikiTags[ key ]}' -> {key.ToUpper()}" );
      item.Text = item.Text.Replace( key, key.ToUpper() );
    }

    return rc;
  }

  /*
  public bool RemapMrWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all MR with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapElementDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new MediaResourceWikiTag(GetLogger(), _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWiki = new MediaResourceWikiTag(GetLogger(), _configuration);
      newWiki.Set(wiki.GetWikiType().ToLower(), newId.Value.ToString());

      item.Text = item.Text.Replace(wiki.GetWiki(), newWiki.GetWiki());

      mappedWikiTags.Add(wiki.GetWiki(), newWiki.GetWiki().ToUpper());
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      GetLogger().LogInformation($"    remapping '{key.ToUpper()}' -> '{mappedWikiTags[key]}'");
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

    var wiki = new QuestionWikiTag(GetLogger(), _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {
      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWiki = new QuestionWikiTag(GetLogger(), _configuration);
      newWiki.Set(wiki.GetWikiType().ToLower(), newId.Value.ToString());

      item.Text = item.Text.Replace(wiki.GetWiki(), newWiki.GetWiki());

      mappedWikiTags.Add(wiki.GetWiki(), newWiki.GetWiki().ToUpper());
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      GetLogger().LogInformation($"    remapping '{key.ToUpper()}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }

  public bool RemapCrWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all QU with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapCounterDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new CounterWikiTag(GetLogger(), _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {

      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWiki = new CounterWikiTag(GetLogger(), _configuration);
      newWiki.Set(wiki.GetWikiType().ToLower(), newId.Value.ToString());

      item.Text = item.Text.Replace(wiki.GetWiki(), newWiki.GetWiki());

      mappedWikiTags.Add(wiki.GetWiki(), newWiki.GetWiki().ToUpper());
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      GetLogger().LogInformation($"    remapping '{key.ToUpper()}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }

  public bool RemapConstWikiTags(MapNodes item)
  {
    var rc = true;

    // remap all QU with new id's in node text
    var dto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdElementDto);
    var mappedWikiTags = new Dictionary<string, string>();

    var wiki = new ConstantWikiTag(GetLogger(), _configuration);
    while (wiki.HaveWikiTag(item.Text))
    {

      var id = Convert.ToUInt32(wiki.GetWikiArgument1());

      var newId = dto.GetIdTranslation(GetFileName(), id);

      var newWiki = new ConstantWikiTag(GetLogger(), _configuration);
      newWiki.Set(wiki.GetWikiType().ToLower(), newId.Value.ToString());

      item.Text = item.Text.Replace(wiki.GetWiki(), newWiki.GetWiki());

      mappedWikiTags.Add(wiki.GetWiki(), newWiki.GetWiki().ToUpper());
    }

    foreach (var key in mappedWikiTags.Keys)
    {
      GetLogger().LogInformation($"    remapping '{key.ToUpper()}' -> '{mappedWikiTags[key]}'");
      item.Text = item.Text.Replace(key, mappedWikiTags[key]);
    }

    return rc;
  }
  */
}