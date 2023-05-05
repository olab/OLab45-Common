using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Dto.Designer;
using OLabWebAPI.Model;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Designer
{
  public partial class TemplateEndpoint : OlabEndpoint
  {

    public TemplateEndpoint(
      OLabLogger logger,
      IOptions<AppSettings> appSettings,
      OLabDBContext context) : base(logger, appSettings, context)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<MapsDto>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {
      logger.LogDebug($"TemplatesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

      var items = new List<Model.Maps>();
      var total = 0;
      var remaining = 0;

      if (!skip.HasValue)
        skip = 0;

      if (take.HasValue && skip.HasValue)
      {
        items = await dbContext.Maps
          .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
          .Skip(skip.Value)
          .Take(take.Value)
          .OrderBy(x => x.Name)
          .ToListAsync();
        remaining = total - take.Value - skip.Value;
      }
      else
      {
        items = await dbContext.Maps
          .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
          .OrderBy(x => x.Name)
          .ToListAsync();
      }

      total = items.Count;

      if (!skip.HasValue)
        skip = 0;

      items = await dbContext.Maps.Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1).OrderBy(x => x.Name).ToListAsync();
      total = items.Count;

      if (take.HasValue && skip.HasValue)
      {
        items = items.Skip(skip.Value).Take(take.Value).ToList();
        remaining = total - take.Value - skip.Value;
      }

      logger.LogDebug(string.Format("found {0} templates", items.Count));

      IList<MapsDto> dtoList = new MapsMapper(logger).PhysicalToDto(items);
      return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MapNodeLinkTemplateDto Links()
    {
      logger.LogDebug($"TemplatesController.Links()");

      var phys = MapNodeLinks.CreateDefault();

      MapNodeLinkTemplateDto dto = new ObjectMapper.MapNodeLinkTemplate(logger).PhysicalToDto(phys);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MapNodeTemplateDto Nodes()
    {
      logger.LogDebug($"TemplatesController.Nodes()");

      var phys = Model.MapNodes.CreateDefault();
      MapNodeTemplateDto dto = new ObjectMapper.MapNodeTemplate(logger).PhysicalToDto(phys);
      return dto;
    }
  }
}

