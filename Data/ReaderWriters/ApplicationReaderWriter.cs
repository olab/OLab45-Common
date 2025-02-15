using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class ApplicationReaderWriter : ReaderWriter
{

  public static ApplicationReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    return new ApplicationReaderWriter( logger, dbContext );
  }

  public ApplicationReaderWriter(
    IOLabLogger logger,
  OLabDBContext dbContext) : base( logger, dbContext )
  {
  }

  /// <summary>
  /// Create new Application
  /// </summary>
  /// <param name="name">Application name</param>
  /// <returns>SystemApplications</returns>
  public async Task<SystemApplications> CreateAsync(string name)
  {
    var newPhys = new SystemApplications { Name = name };
    var existingPhys = await GetAsync( name );

    if ( existingPhys == null )
    {
      GetLogger().LogInformation( $"creating SystemApplications '{newPhys.Name}'s" );
      GetDbContext().SystemApplications.Add( newPhys );
      GetDbContext().SaveChanges();
    }
    else
      newPhys = existingPhys;

    return newPhys;
  }

  /// <summary>
  /// Get Application by id
  /// </summary>
  /// <param name="id">Application Id</param>
  /// <returns>SystemApplications</returns>
  public async Task<SystemApplications> GetAsync(string source)
  {
    SystemApplications phys;

    if ( uint.TryParse( source, out var id ) )
      phys = await GetDbContext().SystemApplications.FirstOrDefaultAsync( x => x.Id == id );
    else
      phys = await GetDbContext().SystemApplications.FirstOrDefaultAsync( x => x.Name == source );

    return phys;
  }

  /// <summary>
  /// Get SystemApplications paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<SystemApplications>> GetPagedAsync(int? take, int? skip)
  {
    var response = new OLabAPIPagedResponse<SystemApplications>();

    if ( !take.HasValue && !skip.HasValue )
    {
      response.Data = await GetDbContext().SystemApplications.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if ( take.HasValue && skip.HasValue )
    {
      response.Data = await GetDbContext().SystemApplications.Skip( skip.Value ).Take( take.Value ).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = GetDbContext().SystemApplications.Count() - skip.Value - response.Count;
    }

    else
      GetLogger().LogWarning( $"invalid/partial take/skip parameters" );

    return response;
  }

  /// <summary>
  /// Test if Application exists by id
  /// </summary>
  /// <param name="id">Application id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(string source)
  {
    if ( uint.TryParse( source, out var id ) )
      return await GetDbContext().SystemApplications.AnyAsync( x => x.Id == id );
    else
      return await GetDbContext().SystemApplications.AnyAsync( x => x.Name == source );
  }

  public async Task DeleteAsync(string source)
  {
    var phys = await GetAsync( source );
    if ( phys != null )
    {
      GetDbContext().SystemApplications.Remove( phys );
      await GetDbContext().SaveChangesAsync();
    }
  }
}
