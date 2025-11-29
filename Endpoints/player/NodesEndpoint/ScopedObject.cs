using DocumentFormat.OpenXml.Spreadsheet;
using OLab.Access.Interfaces;
using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Contracts;
using OLab.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class NodesEndpoint : OLabEndpoint
{
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(
    uint nodeId,
    IOLabAuthorization auth,
    Dictionary<string, IEnumerable<string>> headers)
  {
    GetLogger().LogInformation( $"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})" );
    return await GetScopedObjectsAsync( nodeId, auth, headers, false );
  }

  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
    uint nodeId,
    IOLabAuthorization auth,
    Dictionary<string, IEnumerable<string>> headers)
  {
    GetLogger().LogInformation( $"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})" );
    return await GetScopedObjectsAsync( nodeId, auth, headers, true );
  }

  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
    uint id,
    IOLabAuthorization auth,
    Dictionary<string, IEnumerable<string>> headers,
    bool enableWikiTranslation)
  {
    GetLogger().LogInformation( $"NodesController.GetScopedObjectsAsync(uint nodeId={id})" );

    var node = GetSimple( GetDbContext(), id );
    if ( node == null )
      throw new OLabObjectNotFoundException( Utils.Constants.ScopeLevelNode, id );

    var phys = new ScopedObjects(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      _fileStorageModule );
    await phys.LoadScopedObjectsFromDatabaseAsync( Constants.ScopeLevelNode, node.Id );

    phys.ConstantsPhys.Add( new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantNodeId,
      ImageableId = node.Id,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.ASCII.GetBytes( node.Id.ToString() )
    } );

    phys.ConstantsPhys.Add( new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantNodeName,
      ImageableId = node.Id,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.UTF8.GetBytes( node.Title )
    } );

    phys.ConstantsPhys.Add( new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantSystemTime,
      ImageableId = 1,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.ASCII.GetBytes( DateTime.UtcNow.ToString() + " UTC" )
    } );

    var sessionId = headers.TryGetValue( "OlabSessionId", out var sessionIds ) ?
      sessionIds.FirstOrDefault() ??
      string.Empty : string.Empty;

    SessionStatistics sessionStats = await _sessionEndpoint.GetSessionStats( sessionId );

    phys.ConstantsPhys.Add(
      new SystemConstants
      {
        Id = 0,
        Name = "SessionId",
        Value = Encoding.ASCII.GetBytes( sessionStats.SessionId ),
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    phys.ConstantsPhys.Add(
      new SystemConstants
      {
        Id = 0,
        Name = "SessionTimeStamp",
        Value = Encoding.ASCII.GetBytes( sessionStats.SessionStart.HasValue ? $"{sessionStats.SessionStart.Value.ToString()} UTC" : "<unknown>" ),
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    phys.ConstantsPhys.Add(
      new SystemConstants
      {
        Id = 0,
        Name = "SessionDuration",
        Value = Encoding.ASCII.GetBytes( Math.Floor( sessionStats.SessionDuration.TotalSeconds ).ToString() ),
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    phys.ConstantsPhys.Add(
      new SystemConstants
      {
        Id = 0,
        Name = "NodesVisited",
        Value = Encoding.ASCII.GetBytes( sessionStats.NodeCount.ToString() ),
        ImageableId = 1,
        ImageableType = "Server",
        IsSystem = 1,
        CreatedAt = DateTime.UtcNow
      } );

    var builder = new ObjectMapper.ScopedObjectsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation );

    var dto = builder.PhysicalToDto( phys );
    return dto;
  }
}