using OLab.Api.Importer;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Import.OLab3.Dtos;


public class XmlMapVpdDto : XmlImportDto<XmlMapVpds>
{
  private readonly Api.ObjectMapper.MapVpd _mapper;

  public XmlMapVpdDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      Importer.DtoTypes.XmlMapVpdDto,
      "map_vpd.xml" )
  {
    _mapper = new Api.ObjectMapper.MapVpd( GetLogger(), GetDbContext(), GetWikiProvider() );
  }

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="physicalFileFolder">Physical folder for import files</param>
  /// <param name="displayProgressMessage">Display progress messages</param>
  /// <returns></returns>
  public override async Task<bool> LoadAsync(
    string physicalFileFolder,
    bool displayProgressMessage = true)
  {
    var rc = true;

    try
    {
      var physicalModuleFile = GetFileModule().BuildPath(
        physicalFileFolder,
        GetFileName() );

      GetLogger().LogInformation( $"Loading {physicalModuleFile}" );

      using var moduleFileStream = new MemoryStream();
      if ( await GetFileModule().ReadFileAsync(
        moduleFileStream,
        physicalModuleFile,
        new System.Threading.CancellationToken() ) )
        _phys = DynamicXml.Load( moduleFileStream );
      else
      {
        GetLogger().LogInformation( $"File {physicalFileFolder}{GetFileModule().GetFolderSeparator()}{GetFileName()} does not exist" );
        return false;
      }

      dynamic outerElements = GetElements( GetXmlPhys() );
      var record = 0;

      foreach ( var innerElements in outerElements )
      {
        try
        {
          ++record;
          var elements = (IEnumerable<dynamic>)innerElements.Elements();
          xmlImportElementSets.Add( elements );

          var item = _mapper.ElementsToPhys( elements );

          var phys = new XmlMapVpd
          {
            Id = item.Id,
            MapId = item.MapId,
            VpdTypeId = item.VpdTypeId
          };

          GetLogger().LogInformation( $"  loaded '{phys.Id}'" );

          GetModel().Data.Add( phys );
          record++;
        }
        catch ( Exception ex )
        {
          GetLogger().LogError( ex, $"error loading '{GetFileName()}' record #{record}: {ex.Message}" );
        }

        GetLogger().LogInformation( $"imported {xmlImportElementSets.Count()} {GetFileName()} objects" );

      }

      // delete data file
      await GetFileModule().DeleteFileAsync( physicalModuleFile );
    }
    catch ( Exception ex )
    {
      GetLogger().LogError( ex, $"Load error: {ex.Message}" );
      rc = false;
    }

    return rc;
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.map_vpd.Elements();
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

    GetDbContext().MapVpds.Add( item );
    GetDbContext().SaveChanges();

    CreateIdTranslation( oldId, item.Id );

    return true;
  }

}