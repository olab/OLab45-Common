using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class ScriptsMapper : OLabMapper<SystemScripts, ScriptsDto>
{
  public ScriptsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }
}