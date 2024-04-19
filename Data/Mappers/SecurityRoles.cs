using OLab.Api.Common;
using OLab.Api.Model;
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

  public override SecurityRolesDto PhysicalToDto(Model.SecurityRoles phys, SecurityRolesDto source)
  {
    base.PhysicalToDto(phys, source);

    source.Acl = SecurityUsers.BitMaskToAclString(phys.Acl2);

    return source;
  }

  public override Model.SecurityRoles DtoToPhysical(SecurityRolesDto dto, Model.SecurityRoles phys)
  {
    base.DtoToPhysical(dto, phys);

    phys.Acl2 = SecurityUsers.AclStringToBitMask(dto.Acl);

    return phys;
  }


}