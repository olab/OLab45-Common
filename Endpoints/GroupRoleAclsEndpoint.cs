using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OLab.Access.Interfaces;
using OLab.Api.Common;
using OLab.Api.Common.Exceptions;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Contracts;
using OLab.Data.Interface;
using OLab.Data.Mappers;
using OLab.Data.ReaderWriters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class GroupRoleAclsEndpoint : OLabEndpoint
{
  private readonly GroupRoleAclReaderWriter _readerWriter;
  private readonly IOLabMapper<GrouproleAcls, GroupRoleAclDto> _mapper;

  public GroupRoleAclsEndpoint(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IOLabModuleProvider<IFileStorageModule> fileStorageProvider) : base(
      logger,
      configuration,
      dbContext,
      wikiTagProvider,
      fileStorageProvider )
  {
    _readerWriter = GroupRoleAclReaderWriter.Instance( logger, dbContext );
    _mapper = new GroupRoleAclMapper(
      GetLogger(),
      GetDbContext() );
  }

  public async Task<GroupRoleAclDto> CreateAsync(
    IOLabAuthorization auth,
    GroupRoleAclDto dto)
  {
    GetLogger().LogInformation( JsonConvert.SerializeObject( dto ) );

    // test if user has access to add users.
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    var newPhys = _mapper.DtoToPhysical( dto );

    var phys = await _readerWriter.CreateAsync( newPhys, true );
    GetLogger().LogInformation( GrouproleAcls.TruncateToJsonObject( phys, 1 ) );

    _mapper.PhysicalToDto( phys, dto );

    return dto;
  }

  /// <summary>
  /// Edits a group role acl
  /// </summary>
  /// <param name="model">Object to delete</param>
  public async Task<GroupRoleAclDto> EditAsync(
    IOLabAuthorization auth,
    GroupRoleAclDto dto)
  {
    GetLogger().LogInformation( $"editing acl {dto.Id.Value}" );

    GetLogger().LogInformation( JsonConvert.SerializeObject( dto ) );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.GetAsync( dto.Id.Value );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "GrouproleAcl", dto.Id.Value );

    _mapper.DtoToPhysical( dto, phys );
    await _readerWriter.EditAsync( phys, true );

    GetLogger().LogInformation( JsonConvert.SerializeObject( phys ) );

    return dto;
  }

  /// <summary>
  /// Deletes a group role acl
  /// </summary>
  /// <param name="model">Object to delete</param>
  public async Task DeleteAsync(
    IOLabAuthorization auth,
    uint id)
  {
    GetLogger().LogInformation( $"deleting acl {id}" );

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    var phys = await _readerWriter.GetAsync( id );
    if ( phys == null )
      throw new OLabObjectNotFoundException( "GrouproleAcl", id );

    await _readerWriter.DeleteAsync( id, true );
  }

  /// <summary>
  /// Get single object
  /// </summary>
  /// <param name="auth"></param>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<IList<GroupRoleAclDto>> GetAsync(
    IOLabAuthorization auth,
    GroupRoleAclReadRequest model)
  {
    var groupRoleAclsPhys = new List<GrouproleAcls>();

    // test if user has access 
    if ( !await auth.IsSystemSuperuserAsync() )
      throw new OLabUnauthorizedException();

    if ( model.GroupId == 0 )
      model.GroupId = null;

    if ( model.RoleId == 0 )
      model.RoleId = null;

    // no criteria, so query all
    if ( !model.GroupId.HasValue &&
         !model.RoleId.HasValue &&
         (model.MapIds.Count == 0) &&
         (model.AppIds.Count == 0) &&
         (model.NodeIds.Count == 0) )
      groupRoleAclsPhys.AddRange( await _readerWriter.GetAsync() );

    else
    {
      // query by group and role
      if ( (model.MapIds.Count == 0) &&
           (model.AppIds.Count == 0) &&
           (model.NodeIds.Count == 0) )
        groupRoleAclsPhys.AddRange( await _readerWriter.GetListAsync(
          model.GroupId,
          model.RoleId ) );

      // query by node
      else
      {
        if ( model.NodeIds.Count > 0 )
          groupRoleAclsPhys.AddRange( await _readerWriter.GetListAsync(
            model.GroupId,
            model.RoleId,
            Constants.ScopeLevelNode,
            model.NodeIds ) );

        // query by map
        if ( model.MapIds.Count > 0 )
          groupRoleAclsPhys.AddRange( await _readerWriter.GetListAsync(
            model.GroupId,
            model.RoleId,
            Constants.ScopeLevelMap,
            model.MapIds ) );

        // query by application
        if ( model.AppIds.Count > 0 )
          groupRoleAclsPhys.AddRange( await _readerWriter.GetListAsync(
            model.GroupId,
            model.RoleId,
            Constants.ScopeLevelApp,
            model.AppIds ) );
      }

    }

    var itemsDto = _mapper.PhysicalToDto( groupRoleAclsPhys );
    return itemsDto;
  }
}

