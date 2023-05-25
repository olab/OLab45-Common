using OLabWebAPI.Data.Exceptions;
using OLabWebAPI.Data.Interface;
using OLabWebAPI.Model;
using System.Text;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLabWebAPI.Dto.ScopedObjectsNewDto> GetScopedObjectsRawAsync(IOLabAuthentication auth, uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetScopedObjectsRawAsync");

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
    public async Task<OLabWebAPI.Dto.ScopedObjectsNewDto> GetScopedObjectsAsync(IOLabAuthentication auth, uint id)
    {
      logger.LogDebug($"{auth.GetUserContext().UserId}: MapsEndpoint.GetScopedObjectsAsync");

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
    private async Task<OLabWebAPI.Dto.ScopedObjectsNewDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      Maps map = GetSimple(dbContext, id);
      if (map == null)
        throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

      ScopedObjects phys = await GetScopedObjectsAllAsync(map.Id, Utils.Constants.ScopeLevelMap);

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

      var builder = new ObjectMapper.ScopedObjectsNew(logger, enableWikiTranslation);
      Dto.ScopedObjectsNewDto dto = builder.PhysicalToDto(phys);

      return dto;
    }

  }
}
