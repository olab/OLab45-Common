using DocumentFormat.OpenXml.EMMA;
using OLab.Api.Models;
using OLab.Api.Utils;
using OLab.Data;
using OLab.Data.Dtos;
using OLab.Data.Exceptions;
using OLab.Data.Mappers;
using OLab.Data.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class NodesEndpoint : OLabEndpoint
  {

    public async Task<ScopedObjectsDto> GetScopedObjectsRawAsync(uint nodeId)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, false);
    }

    public async Task<ScopedObjectsDto> GetScopedObjectsAsync(uint nodeId)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, true);
    }

    public async Task<ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      Logger.LogInformation($"NodesController.GetScopedObjectsAsync(uint nodeId={id})");

      var node = GetSimple(dbContext, id);
      if (node == null)
        throw new OLabObjectNotFoundException(ConstantStrings.ScopeLevelNode, id);

      var phys = new ScopedObjects(
        Logger,
        dbContext,
      _fileStorageModule);
      await phys.AddScopeFromDatabaseAsync(ConstantStrings.ScopeLevelNode, node.Id);

      phys.ConstantsPhys.Add(new SystemConstants
      {
        Id = 0,
        Name = ConstantStrings.ReservedConstantNodeId,
        ImageableId = node.Id,
        ImageableType = ConstantStrings.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(node.Id.ToString())
      });

      phys.ConstantsPhys.Add(new SystemConstants
      {
        Id = 0,
        Name = ConstantStrings.ReservedConstantNodeName,
        ImageableId = node.Id,
        ImageableType = ConstantStrings.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(node.Title)
      });

      phys.ConstantsPhys.Add(new SystemConstants
      {
        Id = 0,
        Name = ConstantStrings.ReservedConstantSystemTime,
        ImageableId = 1,
        ImageableType = ConstantStrings.ScopeLevelNode,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString() + " UTC")
      });

      var builder = new ScopedObjectsMapper(Logger, _wikiTagProvider, enableWikiTranslation);
      var dto = builder.PhysicalToDto(phys);
      return dto;
    }
  }
}