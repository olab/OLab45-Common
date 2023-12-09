using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects.API;
using OLab.Import.OLab3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLab.Import.OLab3.Dtos
{

  public class XmlMapVpdElementDto : XmlImportDto<XmlMapVpdElements>
  {
    private readonly Api.ObjectMapper.MapVpdElement _mapper;

    public XmlMapVpdElementDto(
      IOLabLogger logger,
      Importer importer) : base(
        logger,
        importer,
        Importer.DtoTypes.XmlMapVpdElementDto,
        "map_vpd_element.xml")
    {
      _mapper = new Api.ObjectMapper.MapVpdElement(logger);
    }

    /// <summary>
    /// Extract records from the xml document
    /// </summary>
    /// <param name="importDirectory">Dynamic Xml object</param>
    /// <returns>Sets of element sets</returns>
    public override IEnumerable<dynamic> GetElements(dynamic xmlPhys)
    {
      return (IEnumerable<dynamic>)xmlPhys.map_vpd_element.Elements();
    }

    /// <summary>
    /// Saves import object to database
    /// </summary>
    /// <param name="dtos">All import dtos (for lookups into related objects)</param>
    /// <param name="elements">XML doc as an array of elements</param>
    /// <returns>Success/failure</returns>
    public override bool Save(int recordIndex, IEnumerable<dynamic> elements)
    {
      var phys = _mapper.ElementsToPhys(elements);

      // only support the VPDText type at this time
      if (phys.Key == "VPDText")
      {
        Logger.LogInformation($"Skipped MapVpdElement record of type 'VPDText'");
        return true;
      }

      var item = new SystemConstants();
      var oldId = phys.VpdId;

      item.Id = 0;
      item.ImageableType = "Maps";
      item.Name = phys.Key;
      item.Value = Encoding.ASCII.GetBytes(phys.Value);
      item.CreatedAt = DateTime.Now;
      item.Description = $"Imported from {GetFileName()} id = {phys.Id}.";

      var vpdDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapVpdDto) as XmlMapVpdDto;
      var vpd = vpdDto.GetModel().Data.First(x => x.Id == phys.VpdId);

      var mapDto = GetImporter().GetDto(Importer.DtoTypes.XmlMapDto) as XmlMapDto;
      item.ImageableId = mapDto.GetIdTranslation(GetFileName(), vpd.MapId).Value;

      Context.SystemConstants.Add(item);
      Context.SaveChanges();

      // don't have a name, so save the id as the new name
      item.Name = item.Id.ToString();
      Context.SaveChanges();

      CreateIdTranslation(oldId, item.Id);

      return true;
    }

  }

}