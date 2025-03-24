using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class RoleReaderWriter : ReaderWriter
{
  public static RoleReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context)
  {
    return new RoleReaderWriter( logger, context );
  }

  public RoleReaderWriter(
    IOLabLogger logger,
    OLabDBContext context) : base( logger, context )
  {
  }

  /// <summary>
  /// Create new Role
  /// </summary>
  /// <param name="name">Role name</param>
  /// <returns>Roles</returns>
  public async Task<Roles> CreateAsync(string name)
  {
    var newPhys = new Roles { Name = name };
    var existingPhys = await GetAsync( name );

    if ( existingPhys == null )
    {
      GetLogger().LogInformation( $"creating grpup '{newPhys.Name}'" );

      GetDbContext().Roles.Add( newPhys );
      GetDbContext().SaveChanges();
    }
    else
      newPhys = existingPhys;

    return newPhys;
  }

  /// <summary>
  /// Get group by id
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns>Roles</returns>
  public async Task<Roles> GetAsync(string source)
  {
    Roles phys;

    if ( uint.TryParse( source, out var id ) )
      phys = await GetDbContext().Roles.FirstOrDefaultAsync( x => x.Id == id );
    else
      phys = await GetDbContext().Roles.FirstOrDefaultAsync( x => x.Name == source );

    return phys;
  }

  /// <summary>
  /// Get all items
  /// </summary>
  /// <returns>IList roles</returns>
  public async Task<IList<Roles>> GetAsync()
  {
    var result = await GetRawAsync<Roles>( null, null );
    return result.items.OrderBy( x => x.Name ).ToList();
  }

  /// <summary>
  /// Test if group exists by id
  /// </summary>
  /// <param name="id">Role id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(string source)
  {
    if ( uint.TryParse( source, out var id ) )
      return await GetDbContext().Roles.AnyAsync( x => x.Id == id );
    else
      return await GetDbContext().Roles.AnyAsync( x => x.Name == source );
  }

  public async Task DeleteAsync(string source)
  {
    var phys = await GetAsync( source );
    if ( phys != null )
    {
      GetDbContext().Roles.Remove( phys );
      await GetDbContext().SaveChangesAsync();
    }
  }
}
