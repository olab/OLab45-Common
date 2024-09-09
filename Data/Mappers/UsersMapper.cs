using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using System.Linq;

namespace OLab.Data.Mappers;

public class UsersMapper : OLabMapper<Users, UsersDto>
{
  public UsersMapper(
    IOLabLogger logger,
    OLabDBContext dbContext) : base(logger, dbContext)
  {
  }

  /// <summary>
  /// Convert a physical object to a specific dto. 
  /// </summary>
  /// <remarks>
  /// Allows for derived class specific overrides that 
  /// don't fit well with default implementation
  /// </remarks>
  /// <param name="phys">Physical object</param>
  /// <param name="source">Base dto object</param>
  /// <returns>Dto object</returns>
  public override UsersDto PhysicalToDto(Users phys, UsersDto source)
  {
    var dto = base.PhysicalToDto(phys, source);

    dto.Roles = 
      new UserGroupRolesMapper( GetLogger(), GetDbContext() ).PhysicalToDto( phys.UserGrouproles.ToList() );

    return dto;
  }

}
