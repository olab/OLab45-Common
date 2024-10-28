using Microsoft.CSharp.RuntimeBinder;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OLab.Import.OLab3.Importer;

namespace OLab.Import.OLab3.Dtos;


public class XmlMediaElementsDto : XmlImportDto<XmlMediaElement>
{
  public XmlMediaElementsDto(
    IOLabLogger logger,
    Importer importer) : base(
      logger,
      importer,
      DtoTypes.XmlMediaElementsDto,
      "media_elements.xml")
  { }

  public override string GetLoggerString(IEnumerable<dynamic> elements)
  {
    return $"";
  }

  /// <summary>
  /// Loads the specific import file into a model object
  /// </summary>
  /// <param name="importDirectory">Directory where import file exists</param>
  /// <returns></returns>
  public override async Task<bool> LoadAsync(string extractPath, bool displayProgressMessage = true)
  {
    var result = await base.LoadAsync(extractPath, false);
    var record = 0;

    if (result)
    {
      try
      {
        var outerElements = (IEnumerable<dynamic>)GetXmlPhys().media_elements.media_elements_files.Elements();

        foreach (var element in outerElements)
        {
          try
          {
            ++record;
            dynamic file = element.Value;
            file = Conversions.Base64Decode(element, true);
            GetModel().MediaElementsFiles.Add(file);
            GetLogger().LogInformation($"  loaded '{file}'");
          }
          catch (Exception ex)
          {
            GetLogger().LogError(ex, $"Error loading '{GetFileName()}' media_elements_files record #{record}: {ex.Message}");
          }
        }
      }
      catch (RuntimeBinderException)
      {
        GetLogger().LogWarning($"No media_elements_files records in {GetFileName()}");
      }

      GetLogger().LogInformation($"loaded {GetModel().MediaElementsFiles.Count()} {GetFileName()} MediaElementsFiles objects");

      record = 0;

      try
      {
        var outerElements = (IEnumerable<dynamic>)GetXmlPhys().media_elements.media_elements_avatars.Elements();

        foreach (var element in outerElements)
        {
          try
          {
            record++;
            dynamic file = element.Value;
            file = Conversions.Base64Decode(file);
            GetModel().MediaElementsAvatars.Add(file);
            GetLogger().LogInformation($"  loaded '{file}'");
          }
          catch (Exception ex)
          {
            GetLogger().LogError(ex, $"Error loading '{GetFileName()}' media_elements_avatars record #{++record}: {ex.Message}");
          }

        }
      }
      catch (RuntimeBinderException)
      {
        GetLogger().LogWarning($"No media_elements_avatars records in {GetFileName()}");
      }

      GetLogger().LogInformation($"loaded {GetModel().MediaElementsAvatars.Count()} {GetFileName()} MediaElementsAvatars objects");

    }

    return result;
  }

  /// <summary>
  /// Extract records from the xml document
  /// </summary>
  /// <param name="importDirectory">Dynamic Xml object</param>
  /// <returns>Sets of element sets</returns>
  public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
  {
    return (IEnumerable<dynamic>)xmlPhys.media_elements.Elements();
  }


  /// <summary>
  /// Saves media files to public website directory
  /// </summary>
  /// <param name="elements">XML doc as an array of elements</param>
  /// <returns>Success/failure</returns>
  public override bool SaveToDatabase(
    string relativeImportFolder,
    int recordIndex,
    IEnumerable<dynamic> elements)
  {
    var rc = true;

    try
    {
      var relativeMediaSourceDirectory = GetFileModule().GetImportMediaFilesDirectory(relativeImportFolder);

      var mapDto = GetImporter().GetDto(DtoTypes.XmlMapDto) as XmlMapDto;
      var map = mapDto.GetModel().Data.FirstOrDefault();

      var targetDirectory =
        GetFileModule().GetPublicFileDirectory("Maps", map.Id);

      foreach (var element in elements)
      {
        try
        {
          dynamic fileName = Conversions.Base64Decode(element, true);

          var relativeSourceFile = GetFileModule().BuildPath(relativeMediaSourceDirectory, fileName);

          GetFileModule().MoveFileAsync(
            relativeSourceFile,
            targetDirectory).Wait();
        }
        catch (Exception ex)
        {
          GetLogger().LogError($"Error importing {GetFileName()} '{element.Name}' = '{element.Value}': reason : {ex.Message}");
          rc = false;
        }

      }

    }
    catch (Exception ex)
    {
      GetLogger().LogError($"Error importing {GetFileName()}: reason : {ex.Message}");
      rc = false;
    }

    return rc;
  }
}