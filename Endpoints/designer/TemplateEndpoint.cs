using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Designer
{
  public partial class TemplateEndpoint : OLabEndpoint
  {
    private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;

    public TemplateEndpoint(
      IOLabLogger logger,
      IConfiguration configuration,
      OLabDBContext context,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider) : base(logger, configuration, context)
    {
      _wikiTagProvider = wikiTagProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<OLabAPIPagedResponse<MapsDto>> GetAsync([FromQuery] int? take, [FromQuery] int? skip)
    {
      Logger.LogDebug($"TemplatesController.GetAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

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

      Logger.LogDebug(string.Format("found {0} templates", items.Count));

      var dtoList = new MapsMapper(Logger).PhysicalToDto(items);
      return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MapNodeLinkTemplateDto Links()
    {
      Logger.LogDebug($"TemplatesController.Links()");

      var phys = MapNodeLinks.CreateDefault();

      var dto = new ObjectMapper.MapNodeLinkTemplate(Logger).PhysicalToDto(phys);
      return dto;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MapNodeTemplateDto Nodes()
    {
      Logger.LogDebug($"TemplatesController.Nodes()");

      var phys = Model.MapNodes.CreateDefault();
      var dto = new ObjectMapper.MapNodeTemplate(Logger).PhysicalToDto(phys);
      return dto;
    }
  }
}

