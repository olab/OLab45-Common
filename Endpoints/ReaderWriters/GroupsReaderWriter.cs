using OLab.Api.Model;
using System.Linq;
using System.Threading.Tasks;
using OLab.Api.Data.Interface;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.IO;
using OLab.Common.Interfaces;
using OLab.Api.Data.Exceptions;
using System;

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

  /// <summary>
  /// Reads a record from the database
  /// </summary>
  /// <param name="nameOrId"></param>
  /// <param name="throwIfNotFound"></param>
  /// <returns></returns>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public override async Task<Groups> GetAsync(string nameOrId, bool throwIfNotFound = true)
  {
    Groups phys = null;

    if (uint.TryParse(nameOrId, out var id))
      phys = await dbContext.Groups.FirstOrDefaultAsync(e => e.Id == id);
    else
    {
      var myWriter = new StringWriter();

      // Decode any html encoded string.
      HttpUtility.HtmlDecode(nameOrId, myWriter);

      phys = await dbContext.Groups
        .FirstOrDefaultAsync(e => e.Name == myWriter.ToString());
    }

    if ((phys == null) && throwIfNotFound)
      throw new OLabObjectNotFoundException("Groups", nameOrId);

    return phys;
  }

  /// <summary>
  /// Edit a record
  /// </summary>
  /// <param name="phys"></param>
  /// <param name="throwIfNotFound"></param>
  /// <returns></returns>
  public override async Task<Groups> EditAsync(Groups phys, bool throwIfNotFound = true)
  {
    try
    {
      dbContext.Entry(phys).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
    }
    catch (Exception)
    {
      if (throwIfNotFound)
        throw;
    }

    return phys;

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

    var securityRolePhys = SecurityRoles.CreateDefaultForGroup(
        dbContext,
        phys.Id,
        Utils.Constants.ScopeLevelMap,
        0);

    // add default group security roles for all maps in group
    dbContext.SecurityRoles.AddRange(securityRolePhys);

    return phys;
  }

  /// <summary>
  /// Delete object and related references
  /// </summary>
  /// <param name="phys">Groups object</param>
  public override async Task DeleteAsync(Groups phys, bool throwIfNotFound = true)
  {
    var groupPhys = await GetAsync(phys.Name);

    foreach (var mapGroupPhys in dbContext.MapGroups.Where(x => x.GroupId == phys.Id))
      dbContext.MapGroups.Remove(mapGroupPhys);

    foreach (var userGroupPhys in dbContext.UserGroups.Where(x => x.GroupId == phys.Id))
      dbContext.UserGroups.Remove(userGroupPhys);

    foreach (var securityPhys in dbContext.SecurityRoles.Where(x => x.GroupId == phys.Id))
      dbContext.SecurityRoles.Remove(securityPhys);

    dbContext.Groups.Remove(phys);

    return;
  }

}
