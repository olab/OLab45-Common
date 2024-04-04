using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;

namespace OLab.Api.ObjectMapper;

public class SecurityRoles : OLabMapper<Model.SecurityRoles, SecurityRolesDto>
{
  public SecurityRoles(
    IOLabLogger logger, 
    bool enableWikiTranslation = true) : base(logger)
  {
  }

  public SecurityRoles(
    IOLabLogger logger, 
    WikiTagProvider tagProvider, 
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

}