using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;

namespace OLab.Data.Mappers;

public class ApplicationsMapper : OLabMapper<SystemApplications, ApplicationsDto>
{

  public ApplicationsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext) : base( logger, dbContext )
  {
  }
}