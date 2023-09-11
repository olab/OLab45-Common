using Microsoft.CSharp.RuntimeBinder;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static OLabWebAPI.Importer.Importer;

namespace OLabWebAPI.Importer
{

  public class XmlMediaElementsDto : XmlImportDto<XmlMediaElement>
  {
    public XmlMediaElementsDto(Importer importer) : base(importer, "media_elements.xml") { }

    /// <summary>
    /// Loads the specific import file into a model object
    /// </summary>
    /// <param name="importDirectory">Directory where import file exists</param>
    /// <returns></returns>
    public override bool Load(string importDirectory)
    {
      var result = base.Load(importDirectory);
      var record = 0;

      if (result)
      {
        try
        {
          var outerElements = (IEnumerable<dynamic>)GetXmlPhys().media_elements.media_elements_files.Elements();

          foreach (dynamic element in outerElements)
          {
            try
            {
              ++record;
              dynamic file = element.Value;
              file = Conversions.Base64Decode(element, true);
              GetModel().MediaElementsFiles.Add(file);
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

        record = 0;

        try
        {
          var outerElements = (IEnumerable<dynamic>)GetXmlPhys().media_elements.media_elements_avatars.Elements();

          foreach (dynamic element in outerElements)
          {
            try
            {
              record++;
              dynamic file = element.Value;
              file = Conversions.Base64Decode(file);
              GetModel().MediaElementsAvatars.Add(file);
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

        GetLogger().LogDebug($" loaded {GetModel().MediaElementsFiles.Count()} {GetFileName()} MediaElementsFiles objects");
        GetLogger().LogDebug($" loaded {GetModel().MediaElementsAvatars.Count()} {GetFileName()} MediaElementsAvatars objects");

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
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      var rc = true;

      try
      {
        var sourceDirectory = Path.Combine(GetImportPackageDirectory(), "media");

        var mapDto = GetImporter().GetDto(DtoTypes.XmlMapDto) as XmlMapDto;
        Model.Maps map = mapDto.GetModel().Data.FirstOrDefault();

        var targetDirectory = GetPublicFileDirectory("Maps", map.Id);
        Directory.CreateDirectory(targetDirectory);

        foreach (dynamic element in elements)
        {
          try
          {
            dynamic fileName = Conversions.Base64Decode(element, true);

            dynamic sourceFileName = Path.Combine(sourceDirectory, fileName);
            dynamic targetFileName = Path.Combine(targetDirectory, fileName);
            File.Copy(sourceFileName, targetFileName, true);

            GetLogger().LogDebug($" copied {GetFileName()} '{fileName}' -> '{targetDirectory}'");

          }
          catch (System.Exception ex)
          {
            GetLogger().LogError($" error importing {GetFileName()} '{element.Name}' = '{element.Value}': reason : {ex.Message}");
            rc = false;
          }

        }

      }
      catch (System.Exception ex)
      {
        GetLogger().LogError($" error importing {GetFileName()}: reason : {ex.Message}");
        rc = false;
      }

      return rc;
    }
  }

}