using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class ConstantsEndpoint : OLabEndpoint
{
  private readonly IOLabMapper<SystemConstants, ConstantsDto> _mapper;

  public ConstantsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context) : base( logger, configuration, context )
  {
    _mapper = new ObjectMapper.ConstantsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider() );
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  private bool Exists(uint id)
  {
    return GetDbContext().SystemConstants.Any( e => e.Id == id );
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="take"></param>
  /// <param name="skip"></param>
  /// <returns></returns>
  public async Task<OLabAPIPagedResponse<ConstantsDto>> GetAsync(
    IOLabAuthorization auth,
    int? take,
    int? skip)
  {
    var physItems = await GetPhysAsync<SystemConstants>( auth, take, skip );

    var dtoResponse = new OLabAPIPagedResponse<ConstantsDto>();
    dtoResponse.Data = _mapper.PhysicalToDto( physItems.Data.OrderBy( x => x.Name ).ToList() );
    dtoResponse.Remaining = physItems.Remaining;
    dtoResponse.Count = physItems.Count;

    var maps = GetDbContext().Maps.Select( x => new IdName() { Id = x.Id, Name = x.Name } ).ToList();
    var nodes = GetDbContext().MapNodes.Select( x => new IdName() { Id = x.Id, Name = x.Title } ).ToList();
    var servers = GetDbContext().Servers.Select( x => new IdName() { Id = x.Id, Name = x.Name } ).ToList();

    foreach ( var dto in dtoResponse.Data )
      dto.ParentInfo = FindParentInfo( dto.ImageableType, dto.ImageableId, maps, nodes, servers );

    return dtoResponse;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<ConstantsDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {
    GetLogger().LogInformation( $"{auth.OLabUser.Id}: ConstantsEndpoint.ReadAsync" );

    if ( !Exists( id ) )
      throw new OLabObjectNotFoundException( "ConstantsPhys", id );

    var phys = await GetDbContext().SystemConstants.FirstAsync( x => x.Id == id );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "SystemConstants", id );

    var dto = _mapper.PhysicalToDto( phys );

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskRead, dto );
    if ( !accessResult )
      throw new OLabUnauthorizedException( "ConstantsPhys", id );

    AttachParentObject( dto );

    return dto;
  }

  /// <summary>
  /// Saves a object edit
  /// </summary>
  /// <param name="id">question id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id,
    ConstantsDto dto)
  {
    GetLogger().LogInformation( $"{auth.OLabUser.Id}: ConstantsEndpoint.PutAsync" );

    dto.ImageableId = dto.ParentInfo.Id;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
    if ( !accessResult )
      throw new OLabUnauthorizedException( "ConstantsPhys", id );

    try
    {
      var builder = new ConstantsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
      var phys = builder.DtoToPhysical( dto );

      phys.UpdatedAt = DateTime.Now;

      GetDbContext().Entry( phys ).State = EntityState.Modified;
      await GetDbContext().SaveChangesAsync();
    }
    catch ( DbUpdateConcurrencyException )
    {
      await GetConstantAsync( id );
    }

  }

  /// <summary>
  /// Create new object
  /// </summary>
  /// <param name="dto">object data</param>
  /// <returns>IActionResult</returns>
  public async Task<ConstantsDto> PostAsync(
    IOLabAuthorization auth,
    ConstantsDto dto)
  {
    GetLogger().LogInformation( $"{auth.OLabUser.Id}: ConstantsEndpoint.PostAsync" );

    dto.ImageableId = dto.ParentInfo.Id != 0 ? dto.ParentInfo.Id : dto.ImageableId;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
    if ( !accessResult )
      throw new OLabUnauthorizedException( "ConstantsPhys", 0 );

    var builder = new ConstantsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );
    var phys = builder.DtoToPhysical( dto );

    phys.CreatedAt = DateTime.Now;

    GetDbContext().SystemConstants.Add( phys );
    await GetDbContext().SaveChangesAsync();

    dto = builder.PhysicalToDto( phys );
    return dto;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    uint id)
  {
    GetLogger().LogInformation( $"{auth.OLabUser.Id}: ConstantsEndpoint.DeleteAsync" );

    if ( !Exists( id ) )
      throw new OLabObjectNotFoundException( "ConstantsPhys", id );

    try
    {
      var phys = await GetConstantAsync( id );
      var dto = new ConstantsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).PhysicalToDto( phys );

      // test if user has access to object
      var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
      if ( !accessResult )
        throw new OLabUnauthorizedException( "ConstantsPhys", id );

      GetDbContext().SystemConstants.Remove( phys );
      await GetDbContext().SaveChangesAsync();

    }
    catch ( DbUpdateConcurrencyException )
    {
      await GetConstantAsync( id );
    }

  }

}
