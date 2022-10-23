using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Interface;
using OLabWebAPI.Utils;
using OLabWebAPI.Dto.Designer;

namespace OLabWebAPI.Endpoints.Designer
{
  public partial class TemplateEndpoint : OlabEndpoint
  {

    public TemplateEndpoint(
      OLabLogger logger,
      OLabDBContext context,
      IOlabAuthentication auth) : base(logger, context, auth)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {
      logger.LogDebug($"TemplatesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var items = new List<Model.Maps>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await context.Maps
          .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
          .Skip(skip.Value)
          .Take(take.Value)
          .OrderBy(x => x.Name)
          .ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await context.Maps
          .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
          .OrderBy(x => x.Name)
          .ToListAsync();
      }

      total = items.Count;

      if (!skip.HasValue)
        skip = 0;

      items = await context.Maps.Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1).OrderBy(x => x.Name).ToListAsync();
      total = items.Count;

      if (take.HasValue && skip.HasValue)
      {
        items = items.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      logger.LogDebug(string.Format("found {0} templates", items.Count));

      var dtoList = new MapsMapper(logger).PhysicalToDto(items);
      return OLabObjectPagedListResult<MapsDto>.Result( dtoList, remaining );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ActionResult Links()
    {
      logger.LogDebug($"TemplatesController.Links()");

      var phys = MapNodeLinks.CreateDefault();

      var dto = new ObjectMapper.MapNodeLinkTemplate(logger).PhysicalToDto(phys);
      return OLabObjectResult<MapNodeLinkTemplateDto>.Result(dto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ActionResult Nodes()
    {
      logger.LogDebug($"TemplatesController.Nodes()");

      var phys = Model.MapNodes.CreateDefault();

      var dto = new ObjectMapper.MapNodeTemplate(logger).PhysicalToDto(phys);
      return OLabObjectResult<MapNodeTemplateDto>.Result(dto);      
    }
  }
}

