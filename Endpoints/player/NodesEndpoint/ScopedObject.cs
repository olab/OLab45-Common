using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Utils;
using OLabWebAPI.Dto.Designer;
using OLabWebAPI.Model.ReaderWriter;
using System;
using System.Text;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class NodesEndpoint : OlabEndpoint
  {

    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(uint nodeId)
    {
      logger.LogDebug($"NodesController.GetScopedObjectsRawAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, false);
    }

    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsAsync(uint nodeId)
    {
      logger.LogDebug($"NodesController.GetScopedObjectsAsync(uint nodeId={nodeId})");
      return await GetScopedObjectsAsync(nodeId, true);
    }

    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      logger.LogDebug($"NodesController.GetScopedObjectsAsync(uint nodeId={id})");

      var node = GetSimple(dbContext, id);
      if (node == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);      

      var phys = await GetScopedObjectsAllAsync(node.Id, Utils.Constants.ScopeLevelNode);

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

      var builder = new ObjectMapper.ScopedObjects(logger, enableWikiTranslation);

      var dto = builder.PhysicalToDto(phys);
      return dto;
    }
  }
}