using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class NodesEndpoint : OLabEndpoint
  {

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint nodeId)
    {
      Logger.LogDebug($"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, false);
    }

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(uint nodeId)
    {
      Logger.LogDebug($"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, true);
    }

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      Logger.LogDebug($"NodesController.GetScopedObjectsAsync(uint nodeId={id})");

      var node = GetSimple(dbContext, id);
      if (node == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

      var scopedObjects = new OLab.Data.BusinessObjects.ScopedObjects(
        Logger,
        dbContext,
        node.Id,
        Utils.Constants.ScopeLevelNode);

      var phys = await scopedObjects.ReadAsync(_fileStorageModule);

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantNodeId,
        ImageableId = node.Id,
        ImageableType = Utils.Constants.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(node.Id.ToString())
      });

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantNodeName,
        ImageableId = node.Id,
        ImageableType = Utils.Constants.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(node.Title)
      });

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantSystemTime,
        ImageableId = 1,
        ImageableType = Utils.Constants.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString() + " UTC")
      });

      var builder = new ObjectMapper.ScopedObjectsMapper(Logger, _wikiTagProvider, enableWikiTranslation);

      var dto = builder.PhysicalToDto(phys);
      return dto;
    }
  }
}