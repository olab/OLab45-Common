using OLab.Access.Interfaces;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Contracts;
using OLab.Data.Interface;
using OLab.Data.Mappers;
using OLab.Data.ReaderWriters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints;

public partial class GroupRoleAclsEndpoint : OLabEndpoint
{
  private readonly GroupRoleAclReaderWriter _readerWriter;
  private readonly GroupRoleAclMapper _mapper;

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

  public async Task<IList<GroupRoleAclDto>> GetAsync(
    IOLabAuthorization auth,
    GroupRoleAclRequest model)
  {
    var groupRoleAclsPhys = new List<GrouproleAcls>();

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
        groupRoleAclsPhys.AddRange( await _readerWriter.GetAsync(
          model.GroupId,
          model.RoleId ) );

      // query by node
      else
      {
        if ( model.NodeIds.Count > 0 )
            groupRoleAclsPhys.AddRange( await _readerWriter.GetAsync(
              model.GroupId,
              model.RoleId,
              Constants.ScopeLevelNode,
              model.NodeIds ) );

        // query by map
        if ( model.MapIds.Count > 0 )
            groupRoleAclsPhys.AddRange( await _readerWriter.GetAsync(
              model.GroupId,
              model.RoleId,
              Constants.ScopeLevelMap,
              model.MapIds ) );

        // query by application
        if ( model.AppIds.Count > 0 )
            groupRoleAclsPhys.AddRange( await _readerWriter.GetAsync(
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

