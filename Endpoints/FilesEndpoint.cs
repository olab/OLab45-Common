using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class FilesEndpoint : OLabEndpoint
{
  private readonly IOLabMapper<SystemFiles, FilesDto> _mapper;

  public FilesEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageProvider )
  {
    _mapper = new FilesMapper( 
      GetLogger(),
      GetDbContext(),
      GetWikiProvider() );
  }

  private bool Exists(uint id)
  {
    return GetDbContext().SystemFiles.Any( e => e.Id == id );
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="take"></param>
  /// <param name="skip"></param>
  /// <returns></returns>
  public async Task<OLabAPIPagedResponse<FilesDto>> GetAsync(
    IOLabAuthorization auth, 
    int? take, 
    int? skip)
  {
    var physItems = await GetPhysAsync<SystemFiles>( auth, take, skip );

    var dtoResponse = new OLabAPIPagedResponse<FilesDto>();
    dtoResponse.Data = _mapper.PhysicalToDto( physItems.Data );
    dtoResponse.Remaining = physItems.Remaining;
    dtoResponse.Count = physItems.Count;

    var maps = _mapsReaderWriter.GetMapIdNames();
    var nodes = _nodesReaderWriter.GetNodeIdNames();
    var servers = GetServerIdNames();

    foreach ( var dto in dtoResponse.Data )
      dto.ParentInfo = FindParentInfo( dto.ImageableType, dto.ImageableId, maps, nodes, servers );

    GetLogger().LogInformation( string.Format( "Found {0} FilesDto", dtoResponse.Data.Count ) );

    return dtoResponse;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<FilesFullDto> GetAsync(
    IOLabAuthorization auth,
    uint id)
  {

    GetLogger().LogInformation( $"FilesController.ReadAsync(uint id={id})" );

    var phys = await GetDbContext().SystemFiles.FirstAsync( x => x.Id == id );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "SystemFiles", id );

    _fileStorageModule.AttachUrls( phys );

    var dto = new FilesFull(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).PhysicalToDto( phys );

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskRead, dto );
    if ( accessResult is UnauthorizedResult )
      throw new OLabUnauthorizedException( "FilesPhys", id );

    AttachParentObject( dto );
    return dto;

  }

  /// <summary>
  /// Saves a file edit
  /// </summary>
  /// <param name="id">file id</param>
  /// <returns>IActionResult</returns>
  public async Task PutAsync(
    IOLabAuthorization auth,
    uint id, FilesFullDto dto)
  {

    GetLogger().LogInformation( $"PutAsync(uint id={id})" );

    dto.ImageableId = dto.ParentInfo.Id;

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
    if ( accessResult is UnauthorizedResult )
      throw new OLabUnauthorizedException( "FilesPhys", id );

    try
    {
      var builder = new FilesFull(
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
  /// Create new file
  /// </summary>
  /// <param name="dto">Physical object to save</param>
  /// <returns>FilesFullDto</returns>
  public async Task<FilesFullDto> PostAsync(
    IOLabAuthorization auth,
    FilesFullDto dto,
    CancellationToken token)
  {
    GetLogger().LogInformation( $"FilesController.PostAsync()" );
    var builder = new FilesFull(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() );

    // test if user has access to object
    var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
    if ( accessResult is UnauthorizedResult )
      throw new OLabUnauthorizedException( "SystemFiles", 0 );

    if ( string.IsNullOrEmpty( dto.Mime ) )
      dto.Mime = MimeTypesMap.GetMimeType( Path.GetFileName( dto.FileName ) );

    var phys = builder.DtoToPhysical( dto );
    phys.CreatedAt = DateTime.Now;

    GetDbContext().SystemFiles.Add( phys );
    await GetDbContext().SaveChangesAsync();

    var physFilePath = _fileStorageModule.GetPublicFileDirectory(
      dto.ImageableType,
      dto.ImageableId,
      dto.FileName );

    await _fileStorageModule.WriteFileAsync(
      dto.GetStream(),
      physFilePath,
      token );

    GetLogger().LogInformation( $"wrote file '{dto.Name}' to '{physFilePath}'. Size: {dto.GetStream().Length}" );

    var newDto = builder.PhysicalToDto( phys );
    return newDto;
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

    GetLogger().LogInformation( $"ConstantsEndpoint.DeleteAsync(uint id={id})" );

    if ( !Exists( id ) )
      throw new OLabObjectNotFoundException( "FilesPhys", id );

    try
    {
      var phys = await GetFileAsync( id );
      if ( phys == null )
        throw new OLabObjectNotFoundException( "SystemFiles", id );

      var dto = new FilesFull(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider() ).PhysicalToDto( phys );

      // test if user has access to object
      var accessResult = await auth.HasAccessAsync( IOLabAuthorization.AclBitMaskWrite, dto );
      if ( accessResult is UnauthorizedResult )
        throw new OLabUnauthorizedException( "SystemFiles", id );

      var filePath = _fileStorageModule.BuildPath(
        OLabFileStorageModule.FilesRoot,
        dto.ImageableType,
        dto.ImageableId,
        dto.FileName );

      await _fileStorageModule.DeleteFileAsync(
        filePath );

      GetDbContext().SystemFiles.Remove( phys );
      await GetDbContext().SaveChangesAsync();
    }
    catch ( DbUpdateConcurrencyException )
    {
      var existingObject = await GetFileAsync( id );
      if ( existingObject == null )
        throw new OLabObjectNotFoundException( "SystemFiles", id );
    }

  }

}

