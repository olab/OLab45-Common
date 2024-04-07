using Microsoft.EntityFrameworkCore;
using OLab.Api.Data.Interface;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OLab.Api.Endpoints.ReaderWriters;

public class PagedResult<P>
{
  public int total;
  public int Remaining;
  public IList<P> Data;
}

public abstract class ReaderWriter<T> where T : class
{
  protected readonly OLabDBContext dbContext;
  protected readonly IOLabLogger logger;

  public abstract Task<T> GetAsync(string nameOrId);
  public abstract Task<T> EditAsync(T phys);
  public abstract Task DeleteAsync(T phys);

  protected ReaderWriter(IOLabLogger logger, OLabDBContext context)
  {
    dbContext = context;
    this.logger = logger;
  }

  /// <summary>
  /// Creates an object.  Assumes in a transaction
  /// </summary>
  /// <param name="dbContext">OLabContext</param>
  /// <param name="phys">Object to add</param>
  /// <returns>Created object</returns>
  public virtual async Task<T> CreateAsync(
    IOLabAuthorization auth,
    T phys,
    CancellationToken token = default)
  {
    dbContext.Set<T>().Add(phys);
    await dbContext.SaveChangesAsync(token);

    return phys;
  }

  public async virtual Task<PagedResult<T>> GetAsync(
    IOLabAuthorization auth,
    int? skip,
    int? take)
  {
    var groupsPhys = new List<T>();
    if (!skip.HasValue)
      skip = 0;

    var total = await dbContext.Set<T>().CountAsync();

    int remaining;
    if (take.HasValue && skip.HasValue)
    {
      groupsPhys = await dbContext.Set<T>()
        .Skip(skip.Value)
        .Take(take.Value)
        .ToListAsync();
      remaining = total - take.Value - skip.Value;
    }
    else
    {
      groupsPhys = dbContext.Set<T>()
        .ToList();
      remaining = 0;
    }

    return new PagedResult<T>
    {
      Data = groupsPhys,
      total = total,
      Remaining = remaining
    };
  }

}
