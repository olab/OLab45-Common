using DocumentFormat.OpenXml.EMMA;
using OLab.Api.Data.Exceptions;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Data;
using OLab.Data.BusinessObjects.API;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class NodesEndpoint : OLabEndpoint
  {

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint nodeId)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, false);
    }

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(uint nodeId)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, true);
    }

    public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={id})");

      var node = GetSimple(dbContext, id);
      if (node == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

      var phys = new ScopedObjects(
        Logger,
        dbContext,
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
        Value = Encoding.ASCII.GetBytes(node.Title)
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

      var builder = new ObjectMapper.ScopedObjectsMapper(Logger, _wikiTagProvider, enableWikiTranslation);

      var dto = builder.PhysicalToDto(phys);
      return dto;
    }
  }
}