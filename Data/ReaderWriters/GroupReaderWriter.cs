using OLab.Api.Model;
using System.Linq;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OLab.Api.Common;

namespace OLab.Data.ReaderWriters;

public class GroupReaderWriter : ReaderWriter
{
  public static GroupReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context)
  {
    return new GroupReaderWriter(logger, context);
  }

  public GroupReaderWriter(
    IOLabLogger logger,
    OLabDBContext context) : base(logger, context)
  {
  }

  /// <summary>
  /// Create new Group
  /// </summary>
  /// <param name="name">Group name</param>
  /// <returns>Groups</returns>
  public async Task<Groups> CreateAsync(string name)
  {
    var newPhys = new Groups { Name = name };
    var existingPhys = await GetAsync(name);

    if (existingPhys == null)
    {
      _logger.LogInformation($"creating role '{newPhys.Name}'s");
      _context.Groups.Add(newPhys);
      _context.SaveChanges();
    }
    else
      newPhys = existingPhys;

    return newPhys;
  }

  /// <summary>
  /// Get group by id
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>Groups</returns>
  public async Task<Groups> GetAsync(string source)
  {
    Groups phys;

    if (uint.TryParse(source, out uint id))
      phys = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
    else
      phys = await _context.Groups.FirstOrDefaultAsync(x => x.Name == source);

    return phys;
  }

  /// <summary>
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<Groups>> GetPagedAsync(int? take, int? skip)
  {
    var response = new OLabAPIPagedResponse<Groups>();

    if (!take.HasValue && !skip.HasValue)
    {
      response.Data = await _context.Groups.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if (take.HasValue && skip.HasValue)
    {
      response.Data = await _context.Groups.Skip(skip.Value).Take(take.Value).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = _context.Groups.Count() - skip.Value - response.Count;
    }

    else
      _logger.LogWarning($"invalid/partial take/skip parameters");

    return response;
  }

  /// <summary>
  /// Test if group exists by id
  /// </summary>
  /// <param name="id">Group id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(string source)
  {
    if (uint.TryParse(source, out uint id))
      return await _context.Groups.AnyAsync(x => x.Id == id);
    else
      return await _context.Groups.AnyAsync(x => x.Name == source);
  }

  public async Task DeleteAsync(string source)
  {
    var phys = await GetAsync(source);
    if (phys != null)
    {
      _context.Groups.Remove(phys);
      await _context.SaveChangesAsync();
    }
  }
}
