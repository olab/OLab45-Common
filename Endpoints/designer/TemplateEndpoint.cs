using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Designer;

public partial class TemplateEndpoint : OLabEndpoint
{
  public TemplateEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider)
    : base(
        logger,
        configuration,
        context,
        wikiTagProvider,
        fileStorageProvider)
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
    GetLogger().LogInformation($"TemplatesController.ReadAsync([FromQuery] int? take={take}, [FromQuery] int? skip={skip})");

    var items = new List<Model.Maps>();
    var total = 0;
    var remaining = 0;

    if (!skip.HasValue)
      skip = 0;

    if (take.HasValue && skip.HasValue)
    {
      items = await GetDbContext().Maps
        .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
        .Skip(skip.Value)
        .Take(take.Value)
        .OrderBy(x => x.Name)
        .ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      items = await GetDbContext().Maps
        .Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1)
        .OrderBy(x => x.Name)
        .ToListAsync();
    }

    total = items.Count;

    if (!skip.HasValue)
      skip = 0;

    items = await GetDbContext().Maps.Where(x => x.IsTemplate.HasValue && x.IsTemplate.Value == 1).OrderBy(x => x.Name).ToListAsync();
    total = items.Count;

    if (take.HasValue && skip.HasValue)
    {
      items = items.Skip(skip.Value).Take(take.Value).ToList();
      remaining = total - take.Value - skip.Value;
    }

    GetLogger().LogInformation(string.Format("found {0} templates", items.Count));

    var dtoList = new MapsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(items);
    return new OLabAPIPagedResponse<MapsDto> { Data = dtoList, Remaining = remaining, Count = total };
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public MapNodeLinkTemplateDto Links()
  {
    GetLogger().LogInformation($"TemplatesController.Links()");

    var phys = MapNodeLinks.CreateDefault();

    var dto = new MapNodeLinkTemplate(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys);
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public MapNodeTemplateDto Nodes()
  {
    GetLogger().LogInformation($"TemplatesController.Nodes()");

    var phys = MapNodes.CreateDefault();
    var dto = new MapNodeTemplate(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider()).PhysicalToDto(phys);
    return dto;
  }
}

