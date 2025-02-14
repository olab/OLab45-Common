using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
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

  public virtual async Task<(IList<T> items, int total, int remaining)> GetAsync<T>(int? skip, int? take) where T : class
  {
    var items = new List<T>();

    var total = 0;
    var remaining = 0;

    if ( !skip.HasValue )
      skip = 0;

    if ( take.HasValue && skip.HasValue )
    {
      var tmp = GetDbContext().Set<T>().Skip( skip.Value ).Take( take.Value );
      GetLogger().LogInformation( tmp.ToString() );

      items = await GetDbContext().Set<T>().Skip( skip.Value ).Take( take.Value ).ToListAsync();
      total = await GetDbContext().Set<T>().CountAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      items = await GetDbContext().Set<T>().ToListAsync();
      total = items.Count;
    }

    GetLogger().LogInformation( $"found {items.Count} {typeof( T ).Name} items" );
    return ( items, total, remaining);
  }

}
