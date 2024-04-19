using OLab.Api.Data.Exceptions;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Data;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player;

public partial class MapsEndpoint : OLabEndpoint
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsRawAsync(IOLabAuthorization auth, uint id)
  {
    Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetScopedObjectsRawAsync");

    // test if user has access to map.
    if (!auth.HasAccess(SecurityRoles.Read, Utils.Constants.ScopeLevelMap, id))
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

    var result = await GetScopedObjectsAsync(id, false);
    return result;
  }

  /// <summary>
  /// Gets scoped objects for a given map
  /// </summary>
  /// <param name="id">Map Id</param>
  /// <returns>ScopedObjectsMapper dto</returns>
  public async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(IOLabAuthorization auth, uint id)
  {
    // test if user has access to map.
    if (!auth.HasAccess(SecurityRoles.Read, Utils.Constants.ScopeLevelMap, id))
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

    var result = await GetScopedObjectsAsync(id, true);
    return result;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="id"></param>
  /// <param name="enableWikiTranslation"></param>
  /// <returns></returns>
  private async Task<Dto.ScopedObjectsDto> GetScopedObjectsAsync(
    uint id,
    bool enableWikiTranslation)
  {
    var map = GetSimple(dbContext, id);
    if (map == null)
      throw new OLabObjectNotFoundException(Utils.Constants.ScopeLevelMap, id);

    var phys = new ScopedObjects(
      Logger,
      dbContext,
      fileStorageModule);
    await phys.AddScopeFromDatabaseAsync(Constants.ScopeLevelMap, map.Id);

    // add map-level derived constants
    phys.ConstantsPhys.Add(new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantMapId,
      ImageableId = map.Id,
      ImageableType = Utils.Constants.ScopeLevelMap,
      IsSystem = 1,
      Value = Encoding.ASCII.GetBytes(map.Id.ToString())
    });

    phys.ConstantsPhys.Add(new SystemConstants
    {
      Id = 0,
      Name = Utils.Constants.ReservedConstantMapName,
      ImageableId = map.Id,
      ImageableType = Utils.Constants.ScopeLevelMap,
      IsSystem = 1,
      Value = Encoding.UTF8.GetBytes(map.Name)
    });

    var builder = new ObjectMapper.ScopedObjectsMapper(Logger, _wikiTagProvider, enableWikiTranslation);

    var dto = builder.PhysicalToDto(phys);
    dto.Dump(Logger);

    return dto;
  }

}
