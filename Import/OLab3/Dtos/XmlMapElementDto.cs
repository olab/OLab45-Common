using OLab.Common.Interfaces;
using OLab.Import.OLab3.Model;
using System.Collections.Generic;
using System.IO;

namespace OLab.Import.OLab3.Dtos
{

    public class XmlMapElementDto : XmlImportDto<XmlMapElements>
    {
        private readonly Api.ObjectMapper.Files _mapper;
        public XmlMapElementDto(
          IOLabLogger logger,
          Importer importer) : base(
            logger,
            importer,
            Importer.DtoTypes.XmlMapElementDto,
            "map_element.xml")
        {
            _mapper = new Api.ObjectMapper.Files(logger);
        }

        /// <summary>
        /// Extract records from the xml document
        /// </summary>
        /// <param name="importDirectory">Dynamic Xml object</param>
        /// <returns>Sets of element sets</returns>
        public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
        {
            return (IEnumerable<dynamic>)xmlPhys.map_element.Elements();
        }

        /// Saves import object to database
        /// </summary>
        /// <param name="dtos">All import dtos (for lookups into related objects)</param>
        /// <param name="elements">XML doc as an array of elements</param>
        /// <returns>Success/failure</returns>
        public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
        {
            var item = _mapper.ElementsToPhys(elements);
            var oldId = item.Id;

            item.Id = 0;

            var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
            item.ImageableId = mapDto.GetIdTranslation(GetFileName(), item.ImageableId).Value;
            item.ImageableType = "Maps";

            //if (!GetFileModule().FileExists(GetMediaDirectory(), Path.GetFileName(item.Path)))
            //  Logger.LogWarning(GetFileName(), 0, $"media file '{item.Path}' does not exist in import package");

            item.Path = Path.GetFileName(item.Path);

            Context.SystemFiles.Add(item);
            Context.SaveChanges();

            CreateIdTranslation(oldId, item.Id);

            return true;
        }
    }

}