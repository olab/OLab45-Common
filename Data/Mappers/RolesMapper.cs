using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Session;
using System;

namespace OLab.Data.Mappers;

public class RolesMapper : OLabMapper<Roles, RolesDto>
{
  public RolesMapper(
    IOLabLogger logger) : base(logger)
  {
  }

}
