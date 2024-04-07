using OLab.Api.Model;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.IO;
using OLab.Common.Interfaces;
using OLab.Api.Data.Exceptions;
using System;

namespace OLab.Api.Endpoints.ReaderWriters;
public class RolesReaderWriter : ReaderWriter<Roles>
{
  public static RolesReaderWriter Instance(IOLabLogger logger, OLabDBContext context)
  {
    return new RolesReaderWriter(logger, context);
  }

  public RolesReaderWriter(IOLabLogger logger, OLabDBContext context) : base(logger, context)
  {
  }

  /// <summary>
  /// Reads a record from the database
  /// </summary>
  /// <param name="nameOrId"></param>
  /// <param name="throwIfNotFound"></param>
  /// <returns></returns>
  /// <exception cref="OLabObjectNotFoundException"></exception>
  public override async Task<Roles> GetAsync(string nameOrId, bool throwIfNotFound = true)
  {
    Roles phys = null;

    if (uint.TryParse(nameOrId, out var id))
      phys = await dbContext.Roles.FirstOrDefaultAsync(e => e.Id == id);
    else
    {
      var myWriter = new StringWriter();

      // Decode any html encoded string.
      HttpUtility.HtmlDecode(nameOrId, myWriter);

      phys = await dbContext.Roles
        .FirstOrDefaultAsync(e => e.Name == myWriter.ToString());
    }

    if ((phys == null) && throwIfNotFound)
      throw new OLabObjectNotFoundException("Roles", nameOrId);

    return phys;
  }

  public override async Task<Roles> EditAsync(Roles phys, bool throwIfNotFound = true)
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
  /// Delete object and related references
  /// </summary>
  /// <param name="phys">Roles object</param>
  public override Task DeleteAsync(Roles phys, bool throwIfNotFound = true)
  {
    foreach (var securityPhys in dbContext.SecurityRoles.Where(x => x.RoleId == phys.Id))
      dbContext.SecurityRoles.Remove(securityPhys);

    dbContext.Roles.Remove(phys);

    return Task.CompletedTask;
  }

}
