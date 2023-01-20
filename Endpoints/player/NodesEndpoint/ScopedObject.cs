using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Model;
using System;
using System.Text;
using System.Threading.Tasks;

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

            MapNodes node = GetSimple(dbContext, id);
            if (node == null)
                throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelNode, id);

            Model.ScopedObjects phys = await GetScopedObjectsAllAsync(node.Id, Utils.Constants.ScopeLevelNode);

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

            Dto.ScopedObjectsDto dto = builder.PhysicalToDto(phys);
            return dto;
        }
    }
}