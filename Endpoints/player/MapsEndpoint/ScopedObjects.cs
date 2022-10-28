using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OLabWebAPI.Model;
using OLabWebAPI.Common;
using System.Text;
using System;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Interface;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(IOlabAuthentication auth, uint id)
    {
      logger.LogDebug($"MapsController.GetScopedObjectsTranslatedAsync(uint id={id})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      return await GetScopedObjectsAsync(id, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsAsync(IOlabAuthentication auth, uint id)
    {
      logger.LogDebug($"MapsController.GetScopedObjectsTranslatedAsync(uint id={id})");

      // test if user has access to map.
      if (!auth.HasAccess("R", Utils.Constants.ScopeLevelMap, id))
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      return await GetScopedObjectsAsync(id, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      var map = GetSimple(context, id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      var phys = await GetScopedObjectsAllAsync(map.Id, Utils.Constants.ScopeLevelMap);

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantMapId,
        ImageableId = map.Id,
        ImageableType = Utils.Constants.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Id.ToString())
      });

      phys.Constants.Add(new SystemConstants
      {
        Id = 0,
        Name = Utils.Constants.ReservedConstantMapName,
        ImageableId = map.Id,
        ImageableType = Utils.Constants.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Name)
      });

      var builder = new ObjectMapper.ScopedObjects(logger, enableWikiTranslation);

      var dto = builder.PhysicalToDto(phys);
      return dto;
    }

  }
}
