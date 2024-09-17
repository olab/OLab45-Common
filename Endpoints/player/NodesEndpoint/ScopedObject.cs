using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Data;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class NodesEndpoint : OLabEndpoint
{

  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint nodeId)
  {
    GetLogger().LogInformation($"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})");
    return await GetScopedObjectsAsync(nodeId, false);
  }

  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(uint nodeId)
  {
    GetLogger().LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})");
    return await GetScopedObjectsAsync(nodeId, true);
  }

  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
    uint id,
    bool enableWikiTranslation)
  {
    GetLogger().LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={id})");

    var node = GetSimple(GetDbContext(), id);
    if (node == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

    var phys = new ScopedObjects(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      _fileStorageModule);
    await phys.AddScopeFromDatabaseAsync(Constants.ScopeLevelNode, node.Id);

    phys.ConstantsPhys.Add(new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantNodeId,
      ImageableId = node.Id,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.ASCII.GetBytes(node.Id.ToString())
    });

    phys.ConstantsPhys.Add(new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantNodeName,
      ImageableId = node.Id,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.UTF8.GetBytes(node.Title)
    });

    phys.ConstantsPhys.Add(new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantSystemTime,
      ImageableId = 1,
      ImageableType = Utils.Constants.ScopeLevelNode,
      IsSystem = 1,
      Value = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString() + " UTC")
    });

    var builder = new ObjectMapper.ScopedObjectsMapper(
      GetLogger(),
      GetDbContext(),
      GetWikiProvider(),
      enableWikiTranslation);

    var dto = builder.PhysicalToDto(phys);
    return dto;
  }
}