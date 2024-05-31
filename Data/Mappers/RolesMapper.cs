﻿using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;

namespace OLab.Data.Mappers;

public class RolesMapper : OLabMapper<Roles, RolesDto>
{
  public RolesMapper(
    IOLabLogger logger) : base(logger)
  {
  }

}
