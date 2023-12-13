using DocumentFormat.OpenXml.EMMA;
using OLab.Api.Data.Interface;
using OLab.Api.Models;
using OLab.Api.Utils;
using OLab.Data;
using OLab.Data.Dtos;
using OLab.Data.Exceptions;
using OLab.Data.Mappers;
using OLab.Data.Models;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Api.Endpoints.Player
{
  public partial class MapsEndpoint : OLabEndpoint
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ScopedObjectsDto> GetScopedObjectsRawAsync(IOLabAuthorization auth, uint id)
    {
      Logger.LogInformation($"{auth.UserContext.UserId}: MapsEndpoint.GetScopedObjectsRawAsync");

      // test if user has access to map.
      if (!auth.HasAccess("R", ConstantStrings.ScopeLevelMap, id))
        throw new OLabObjectNotFoundException(ConstantStrings.ScopeLevelMap, id);

      var result = await GetScopedObjectsAsync(id, false);
      return result;
    }

    /// <summary>
    /// Gets scoped objects for a given map
    /// </summary>
    /// <param name="id">Map Id</param>
    /// <returns>ScopedObjectsMapper dto</returns>
    public async Task<ScopedObjectsDto> GetScopedObjectsAsync(IOLabAuthorization auth, uint id)
    {
      // test if user has access to map.
      if (!auth.HasAccess("R", ConstantStrings.ScopeLevelMap, id))
        throw new OLabObjectNotFoundException(ConstantStrings.ScopeLevelMap, id);

      var result = await GetScopedObjectsAsync(id, true);
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    private async Task<ScopedObjectsDto> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      var map = GetSimple(dbContext, id);
      if (map == null)
        throw new OLabObjectNotFoundException(ConstantStrings.ScopeLevelMap, id);

      var phys = new ScopedObjects(
        Logger,
        dbContext,
        _fileStorageModule);
      await phys.AddScopeFromDatabaseAsync(ConstantStrings.ScopeLevelMap, map.Id);

      // add map-level derived constants
      phys.ConstantsPhys.Add(new SystemConstants
      {
        Id = 0,
        Name = ConstantStrings.ReservedConstantMapId,
        ImageableId = map.Id,
        ImageableType = ConstantStrings.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Id.ToString())
      });

      phys.ConstantsPhys.Add(new SystemConstants
      {
        Id = 0,
        Name = ConstantStrings.ReservedConstantMapName,
        ImageableId = map.Id,
        ImageableType = ConstantStrings.ScopeLevelMap,
        IsSystem = 1,
        Value = Encoding.ASCII.GetBytes(map.Name)
      });

      var builder = new ScopedObjectsMapper(Logger, _wikiTagProvider, enableWikiTranslation);

      var dto = builder.PhysicalToDto(phys);
      dto.Dump(Logger);

      return dto;
    }

  }
}
