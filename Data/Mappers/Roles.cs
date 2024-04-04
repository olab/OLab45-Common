using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class Roles : OLabMapper<Model.Roles, IdNameDto>
{
  public Roles(
    IOLabLogger logger, 
    bool enableWikiTranslation = true) : base(logger)
  {
  }

  public Roles(
    IOLabLogger logger, 
    WikiTagProvider tagProvider, 
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }
}