using Humanizer;
using NuGet.Packaging;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class MapAuthorizationEndpoint : OLabEndpoint
{
  private readonly MapGrouprolesMapper mapper;
  private readonly MapsReaderWriter mapReader;

  public MapAuthorizationEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      dbContext,
      wikiTagProvider,
      fileStorageProvider)
  {
    mapper = new MapGrouprolesMapper(GetLogger(), GetDbContext());
    mapReader = new MapsReaderWriter(GetLogger(), GetDbContext());
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
  public async Task<IList<MapGrouprolesDto>> DeleteAsync(
    IOLabAuthorization auth,
    MapGrouprolesDto dto,
    CancellationToken token)
  {
    GetLogger().LogInformation($"MapAuthorizationEndpoint.DeleteAsync()");

    // test if user has access to parent object
    var accessResult = await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskWrite,
      Utils.Constants.ScopeLevelMap,
      dto.MapId);

    if (!accessResult)
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, dto.MapId);

    var mapPhys = await mapReader.GetSingleWithGroupRolesAsync(dto.MapId);
    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, dto.MapId);

    var mapGroupPhys = mapPhys.MapGrouproles.FirstOrDefault(x => x.GroupId == dto.GroupId);
    if (mapGroupPhys != null)
    {
      mapPhys.MapGrouproles.Remove(mapGroupPhys);
      GetDbContext().SaveChanges();
    }
    else
      throw new OLabObjectNotFoundException("MapGroup", dto.GroupId);

    return mapper.PhysicalToDto(mapPhys.MapGrouproles.ToList());

  }

  /// <summary>
  /// Add MapGroup to a map
  /// </summary>
  /// <param name="auth">IOLabAuthorization context</param>
  /// <param name="dto">MapGrouprolesDto</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>All groups for map</returns>
  /// <exception cref="OLabUnauthorizedException"></exception>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public async Task<IList<MapGrouprolesDto>> AddAsync(
    IOLabAuthorization auth,
    MapGrouprolesDto dto,
    CancellationToken token)

  {
    GetLogger().LogInformation($"MapAuthorizationEndpoint.AddAsync()");

    // test if user has access to parent object
    var accessResult = await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskWrite,
      Utils.Constants.ScopeLevelMap,
      dto.MapId);

    if (!accessResult)
      throw new OLabUnauthorizedException("Map", dto.MapId);

    var mapPhys = await mapReader.GetSingleWithGroupRolesAsync(dto.MapId);
    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, dto.MapId);

    var reader = GroupReaderWriter.Instance(GetLogger(), GetDbContext());

    // ensure group exists
    if (await reader.ExistsAsync(dto.GroupId.ToString()))
      throw new OLabObjectNotFoundException("Group", dto.GroupId);

    // test if doesn't already exist
    if (!mapPhys.MapGrouproles.Any(x => x.GroupId == dto.GroupId))
    {
      var mapGroupPhys = mapper.DtoToPhysical(dto);
      mapPhys.MapGrouproles.Add(mapGroupPhys);

      GetDbContext().SaveChanges();
    }

    return mapper.PhysicalToDto(mapPhys.MapGrouproles.ToList());

  }

  /// <summary>
  /// Replace map groups with a new set
  /// </summary>
  /// <param name="auth">IOLabAuthorization</param>
  /// <param name="mapId">Map id</param>
  /// <param name="dtos">List of group dtos</param>
  /// <param name="token">CancellationToken</param>
  /// <returns>New list of map groups</returns>
  public async Task<IList<MapGrouprolesDto>> ReplaceAsync(
    IOLabAuthorization auth,
    uint mapId,
    IList<MapGrouprolesDto> dtos,
    CancellationToken token)
  {
    GetLogger().LogInformation($"MapAuthorizationEndpoint.ReplaceAsync()");

    // test if user has access to parent object
    var accessResult = await auth.HasAccessAsync(
      IOLabAuthorization.AclBitMaskWrite,
      Utils.Constants.ScopeLevelMap,
      mapId);

    if (!accessResult)
      throw new OLabUnauthorizedException(Utils.Constants.ScopeLevelMap, mapId);

    var readerWriter = MapsReaderWriter.Instance(GetLogger(), GetDbContext());
    var mapPhys = await readerWriter.GetSingleWithGroupRolesAsync(mapId);

    // ensure map exists
    if (mapPhys == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, mapId);

    mapPhys.MapGrouproles.Clear();

    var mapper = new MapGrouprolesMapper(GetLogger(), GetDbContext());
    var MapGrouprolesPhys = mapper.DtoToPhysical(mapId, dtos);

    mapPhys.MapGrouproles.AddRange(MapGrouprolesPhys);

    GetDbContext().SaveChanges();

    var MapGrouprolesDto = mapper.PhysicalToDto(mapPhys.MapGrouproles.ToList());

    return MapGrouprolesDto;
  }
}
