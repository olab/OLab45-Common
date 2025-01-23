using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class MapNodeLinkTemplate : OLabMapper<Model.MapNodeLinks, MapNodeLinkTemplateDto>
{
  protected readonly bool enableWikiTranslation = false;

  public MapNodeLinkTemplate(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

}
