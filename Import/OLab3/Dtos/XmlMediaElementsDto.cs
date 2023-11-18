using Microsoft.CSharp.RuntimeBinder;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static OLab.Import.OLab3.Importer;

namespace OLab.Import.OLab3.Dtos
{

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

        /// <summary>
        /// Loads the specific import file into a model object
        /// </summary>
        /// <param name="importDirectory">Directory where import file exists</param>
        /// <returns></returns>
        public override bool Load(string extractPath)
        {
            var result = base.Load(extractPath);
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
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Error loading '{GetFileName()}' media_elements_files record #{record}: {ex.Message}");
                        }
                    }
                }
                catch (RuntimeBinderException)
                {
                    Logger.LogWarning($"No media_elements_files records in {GetFileName()}");
                }

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
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Error loading '{GetFileName()}' media_elements_avatars record #{++record}: {ex.Message}");
                        }

                    }
                }
                catch (RuntimeBinderException)
                {
                    Logger.LogWarning($"No media_elements_avatars records in {GetFileName()}");
                }

                Logger.LogInformation($"loaded {GetModel().MediaElementsFiles.Count()} {GetFileName()} MediaElementsFiles objects");
                Logger.LogInformation($"loaded {GetModel().MediaElementsAvatars.Count()} {GetFileName()} MediaElementsAvatars objects");

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
                var sourceDirectory = $"media";

                var mapDto = GetImporter().GetDto(DtoTypes.XmlMapDto) as XmlMapDto;
                var map = mapDto.GetModel().Data.FirstOrDefault();

                var targetDirectory = GetPublicFileDirectory("Maps", map.Id);

                foreach (var element in elements)
                {
                    try
                    {
                        dynamic fileName = Conversions.Base64Decode(element, true);

                        GetImporter().GetFileStorageModule().MoveFileAsync(fileName, sourceDirectory, targetDirectory).Wait();

                        Logger.LogInformation($"Copied {GetFileName()} '{fileName}' -> '{targetDirectory}'");

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error importing {GetFileName()} '{element.Name}' = '{element.Value}': reason : {ex.Message}");
                        rc = false;
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error importing {GetFileName()}: reason : {ex.Message}");
                rc = false;
            }

            return rc;
        }
    }

}