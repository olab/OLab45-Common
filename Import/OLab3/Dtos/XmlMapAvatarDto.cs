using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace OLab.Import.OLab3.Dtos;

public class XmlMapAvatarDto : XmlImportDto<XmlMapAvatars>
{
  private readonly AvatarsMapper _avMapper;
  private readonly FilesMapper _fileMapper;

  public XmlMapAvatarDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapAvatarDto,
      "map_avatar.xml" )
  {
    _avMapper = new AvatarsMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
    _fileMapper = new FilesMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.map_avatar.Elements();
  }

  public SystemFiles CreateAvatarSystemFile(IEnumerable<dynamic> elements, MapAvatars avatar)
  {
    var phys = new SystemFiles
    {
      Id = 0,
      Name = $"Avatar{avatar.Id}",
      Mime = "image/png",
      CreatedAt = DateTime.Now,
      Height = 300,
      HeightType = "px",
      Width = 300,
      WidthType = "px"
    };

    return phys;
  }

  /// <summary>
  /// Saves import object to database
  /// </summary>
  /// <param name="elements">XML doc as an array of elements</param>
  /// <returns>Success/failure</returns>
  public override bool SaveToDatabase(
    string importFolderName,
    int recordIndex,
    IEnumerable<dynamic> elements)
  {
    var avItem = _avMapper.ElementsToPhys( elements );
    var oldId = avItem.Id;

    avItem.Id = 0;

    var mapDto = GetImporter().GetDto( Importer.DtoTypes.XmlMapDto ) as XmlMapDto;
    avItem.MapId = mapDto.GetIdTranslation( GetFileName(), avItem.MapId ).Value;

    GetDbContext().MapAvatars.Add( avItem );
    GetDbContext().SaveChanges();

    GetLogger().LogInformation( $"Saved {GetFileName()} id {avItem.Id}" );

    var fileItem = CreateAvatarSystemFile( elements, avItem );

    fileItem.ImageableId = avItem.MapId;
    fileItem.ImageableType = "Maps";
    fileItem.Path = avItem.Image;

    var physFile =
      GetFileModule().GetPublicFileDirectory(
        fileItem.ImageableType,
        fileItem.ImageableId,
        fileItem.Path );

    if ( !File.Exists( physFile ) )
      GetLogger().LogWarning( GetFileName(), 0, $"media file '{physFile}' does not exist in public directory" );

    GetDbContext().SystemFiles.Add( fileItem );
    GetDbContext().SaveChanges();

    CreateIdTranslation( oldId, fileItem.Id );
    GetLogger().LogInformation( $"Saved SystemFile {fileItem.Id}" );

    return true;
  }

}