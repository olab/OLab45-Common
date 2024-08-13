using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;

namespace OLab.Data.Mappers;

public class UsersMapper : OLabMapper<Users, UsersDto>
{
  public UsersMapper(
    IOLabLogger logger,
    OLabDBContext dbContext) : base(logger, dbContext)
  {
  }

}
