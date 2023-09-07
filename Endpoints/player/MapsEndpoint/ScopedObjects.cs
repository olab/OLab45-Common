using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class MapsEndpoint : OlabEndpoint
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OLab.Api.Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(IOLabAuthentication auth, uint id)
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
    public async Task<OLab.Api.Dto.ScopedObjectsDto> GetScopedObjectsAsync(IOLabAuthentication auth, uint id)
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
    private async Task<OLab.Api.Dto.ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      var map = GetSimple(dbContext, id);
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
