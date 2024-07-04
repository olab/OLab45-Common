using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;

namespace OLab.Data.Mappers;

public class GroupsMapper : OLabMapper<Groups, GroupsDto>
{
  public GroupsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider = null,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

}
