using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class MapNodeTemplate : OLabMapper<Model.MapNodes, MapNodeTemplateDto>
{
  protected readonly bool enableWikiTranslation = false;

  public MapNodeTemplate(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
    this.enableWikiTranslation = enableWikiTranslation;
  }

}
