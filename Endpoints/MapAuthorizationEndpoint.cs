
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class MapAuthorizationEndpoint : OLabEndpoint
{
  private readonly MapGroupsMapper mapper;
  private readonly MapsReaderWriter mapReader;

  public MapAuthorizationEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageProvider)
  {
    mapper = new MapGroupsMapper(Logger, dbContext);
    mapReader = new MapsReaderWriter(Logger, dbContext);
  }

  /// <summary>
  /// Remove group from map
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">Map group to remove</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All groups for map</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<MapGroupsDto>> DeleteAsync(
    IOLabAuthorization auth,
    MapGroupsDto dto,
    CancellationToken token)
  {
    Logger.LogInformation($"AuthorizationEndpoint.DeleteAsync()");

    // test if user has access to parent object
    var accessResult = auth.HasAccess(
      IOLabAuthorization.AclBitMaskExecute,
      Utils.Constants.ScopeLevelMap,
      dto.MapId);

    if (!accessResult)
      throw new OLabUnauthorizedException("Map", dto.MapId);

    var mapPhys = await mapReader.GetSingleWithGroupsAsync(dto.MapId);
    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, dto.MapId);

    var mapGroupPhys = mapPhys.MapGroups.FirstOrDefault(x => x.GroupId == dto.GroupId);
    if (mapGroupPhys != null)
    {
      mapPhys.MapGroups.Remove(mapGroupPhys);
      dbContext.SaveChanges();
    }
    else
      throw new OLabObjectNotFoundException("MapGroup", dto.GroupId);

    return mapper.PhysicalToDto(mapPhys.MapGroups.ToList());

  }

  /// <summary>
  /// Add MapGroup to a map
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">MapGroupsDto</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All groups for map</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<MapGroupsDto>> AddAsync(
    IOLabAuthorization auth,
    MapGroupsDto dto,
    CancellationToken token)

  {
    Logger.LogInformation($"AuthorizationEndpoint.AddAsync()");

    // test if user has access to parent object
    var accessResult = auth.HasAccess(
      IOLabAuthorization.AclBitMaskExecute,
      Utils.Constants.ScopeLevelMap,
      dto.MapId);

    if (!accessResult)
      throw new OLabUnauthorizedException("Map", dto.MapId);

    var mapPhys = await mapReader.GetSingleWithGroupsAsync(dto.MapId);
    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, dto.MapId);

    var reader = GroupRoleReaderWriter.Instance(Logger, dbContext);

    // ensure group exists
    if (await reader.GroupExistsAsync(dto.GroupId))
      throw new OLabObjectNotFoundException("Group", dto.GroupId);

    // test if doesn't already exist
    if (!mapPhys.MapGroups.Any(x => x.GroupId == dto.GroupId))
    {
      var mapGroupPhys = mapper.DtoToPhysical(dto);
      mapPhys.MapGroups.Add(mapGroupPhys);

      dbContext.SaveChanges();
    }

    return mapper.PhysicalToDto(mapPhys.MapGroups.ToList());

  }
}
