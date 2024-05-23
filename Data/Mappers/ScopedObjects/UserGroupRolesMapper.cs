using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.IO;
using System.Linq;

namespace OLab.Api.ObjectMapper;

public class UserGroupRolesMapper : OLabMapper<UserGrouproles, UserGroupRolesDto>
{
  private readonly OLabDBContext dbContext;

  public UserGroupRolesMapper(
    IOLabLogger logger, 
    OLabDBContext dbContext, 
    bool enableWikiTranslation = true) : base(logger)
  {
    this.dbContext = dbContext;
  }

  public UserGroupRolesMapper(
    IOLabLogger logger, 
    WikiTagProvider tagProvider, 
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override UserGrouproles DtoToPhysical(UserGroupRolesDto dto)
  {
    var phys = base.DtoToPhysical(dto);

    phys.Group = dbContext.Groups.FirstOrDefault(x => x.Id == dto.GroupId);
    if (phys.Group == null)
      throw new OLabObjectNotFoundException("Groups", dto.GroupId);

    phys.User = dbContext.Users.FirstOrDefault(x => x.Id == dto.UserId);
    if (phys.User == null)
      throw new OLabObjectNotFoundException("Users", dto.UserId);

    return phys;
  }
}