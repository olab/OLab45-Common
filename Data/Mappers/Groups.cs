using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper;

public class Groups : OLabMapper<Model.Groups, IdNameDto>
{
  public Groups(
    IOLabLogger logger, 
    bool enableWikiTranslation = true) : base(logger)
  {
  }

  public Groups(
    IOLabLogger logger, 
    WikiTagProvider tagProvider, 
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }
}