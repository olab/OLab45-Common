using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Dtos.Session;
using OLab.Data.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class SessionEndpoint : OLabEndpoint
{
  public SessionEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext) : base(logger, configuration, dbContext)
  {
  }

  /// <summary>
  /// Retrieve a map
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>Map</returns>
  public async Task<SessionDto> GetAsync(
    IOLabAuthorization auth,
    string sessionUuid)
  {
    Logger.LogDebug($"{auth.UserContext.UserId}: SessionEndpoint.GetAsync");

    var session = await dbContext.UserSessions
      .Include(session => session.Statements)
      .Include(session => session.UserBookmarks)
      .Include(session => session.UserSessionTraces)
      .FirstOrDefaultAsync(x => x.Uuid == sessionUuid);

    if (session == null)
      throw new OLabObjectNotFoundException("UserSession", sessionUuid);

    var dto = new SessionMapper(Logger).PhysicalToDto(session);

    return dto;
  }
}
