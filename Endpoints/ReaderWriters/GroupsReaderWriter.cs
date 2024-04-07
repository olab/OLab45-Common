using OLab.Api.Model;
using System.Linq;
using System.Threading.Tasks;
using OLab.Api.Data.Interface;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.IO;
using OLab.Common.Interfaces;

namespace OLab.Api.Endpoints.ReaderWriters;
public class GroupsReaderWriter : ReaderWriter<Groups>
{
  public static GroupsReaderWriter Instance(IOLabLogger logger, OLabDBContext context)
  {
    return new GroupsReaderWriter(logger, context);
  }

  public GroupsReaderWriter(IOLabLogger logger, OLabDBContext context) : base(logger, context)
  {
  }

  public override async Task<Groups> GetAsync(string nameOrId)
  {
    if (uint.TryParse(nameOrId, out var id))
      return await dbContext.Groups.FirstOrDefaultAsync(e => e.Id == id);

    var myWriter = new StringWriter();

    // Decode any html encoded string.
    HttpUtility.HtmlDecode(nameOrId, myWriter);

    return await dbContext.Groups
      .FirstOrDefaultAsync(e => e.Name == myWriter.ToString());
  }

  public override Task<Groups> EditAsync(Groups phys)
  {
    throw new System.NotImplementedException();
  }

  /// <summary>
  /// Creates an object.  Assumes in a transaction
  /// </summary>
  /// <param name="dbContext">OLabContext</param>
  /// <param name="phys">Object to add</param>
  /// <returns>Created object</returns>
  public override async Task<Groups> CreateAsync(
    IOLabAuthorization auth,
    Groups phys,
    CancellationToken token = default)
  {
    phys = await base.CreateAsync(auth, phys, token);

    // add default group security roles for all maps in group
    dbContext.SecurityRoles.AddRange(
      SecurityRoles.CreateDefaultRolesForGroup(
        dbContext,
        phys.Id,
        Utils.Constants.ScopeLevelMap,
        0));

    return phys;
  }

  /// <summary>
  /// Delete object and related references
  /// </summary>
  /// <param name="phys">Groups object</param>
  public override Task DeleteAsync(Groups phys)
  {
    foreach (var mapGroupPhys in dbContext.MapGroups.Where(x => x.GroupId == phys.Id))
      dbContext.MapGroups.Remove(mapGroupPhys);

    foreach (var userGroupPhys in dbContext.UserGroups.Where(x => x.GroupId == phys.Id))
      dbContext.UserGroups.Remove(userGroupPhys);

    foreach (var securityPhys in dbContext.SecurityRoles.Where(x => x.GroupId == phys.Id))
      dbContext.SecurityRoles.Remove(securityPhys);

    dbContext.Groups.Remove(phys);

    return Task.CompletedTask;
  }

}
