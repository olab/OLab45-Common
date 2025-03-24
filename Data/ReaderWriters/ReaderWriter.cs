using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public abstract class ReaderWriter
{
  private readonly OLabDBContext _dbContext;
  private readonly IOLabLogger _logger;

  protected OLabDBContext GetDbContext() { return _dbContext; }
  protected IOLabLogger GetLogger() { return _logger; }

  public ReaderWriter(IOLabLogger logger, OLabDBContext context)
  {
    _dbContext = context;
    _logger = logger;
  }

  /// <summary>
  /// Get count of objects
  /// </summary>
  /// <returns></returns>
  public virtual async Task<int> CountAsync<T>() where T : class
  {
    return await GetDbContext().Set<T>().CountAsync();
  }

  /// <summary>
  /// Generic pages reader
  /// </summary>
  /// <typeparam name="T">Object type</typeparam>
  /// <param name="skip">Item skip count</param>
  /// <param name="take">Item take count</param>
  /// <returns></returns>
  public virtual async Task<(IEnumerable<T> items, int count, int remaining)> GetRawAsync<T>(int? skip = null, int? take = null) where T : class
  {
    var items = new List<T>();

    var count = 0;
    var remaining = 0;

    if ( !skip.HasValue )
      skip = 0;

    if ( take.HasValue && skip.HasValue )
    {
      items = await GetDbContext().Set<T>().Skip( skip.Value ).Take( take.Value ).ToListAsync();
      count = items.Count;
      remaining = count - take.Value - skip.Value;
    }
    else
    {
      items = await GetDbContext().Set<T>().ToListAsync();
      count = items.Count;
    }

    GetLogger().LogInformation( $"found {items.Count} {typeof( T ).Name} items" );
    return (items, count, remaining);
  }

}
