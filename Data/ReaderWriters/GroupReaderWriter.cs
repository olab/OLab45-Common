using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class GroupReaderWriter : ReaderWriter
{

  public static GroupReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext dbContext)
  {
    return new GroupReaderWriter(logger, dbContext);
  }

  public GroupReaderWriter(
    IOLabLogger logger,
  OLabDBContext dbContext) : base(logger, dbContext)
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
      GetLogger().LogInformation($"creating role '{newPhys.Name}'s");
      GetDbContext().Groups.Add(newPhys);
      GetDbContext().SaveChanges();

      // add default ACL's for new group
      await GroupRoleAclReaderWriter.Instance(GetLogger(), GetDbContext()).CreateForGroupAsync(newPhys.Id);
      GetDbContext().SaveChanges();

    }
    else
      newPhys = existingPhys;

    return newPhys;
  }

  /// <summary>
  /// Get all groups
  /// </summary>
  /// <returns>Groups</returns>
  public async Task<IList<Groups>> GetAsync()
  {
    var phys = await GetDbContext().Groups.ToListAsync();
    return phys;
  }

  /// <summary>
  /// Get group by id
  /// </summary>
  /// <param name="id">Group Id</param>
  /// <returns>Groups</returns>
  public async Task<Groups> GetAsync(string source)
  {
    Groups phys;

    if (uint.TryParse(source, out var id))
      phys = await GetDbContext().Groups.FirstOrDefaultAsync(x => x.Id == id);
    else
      phys = await GetDbContext().Groups.FirstOrDefaultAsync(x => x.Name == source);

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
      response.Data = await GetDbContext().Groups.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if (take.HasValue && skip.HasValue)
    {
      response.Data = await GetDbContext().Groups.Skip(skip.Value).Take(take.Value).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = GetDbContext().Groups.Count() - skip.Value - response.Count;
    }

    else
      GetLogger().LogWarning($"invalid/partial take/skip parameters");

    return response;
  }

  /// <summary>
  /// Test if group exists by id
  /// </summary>
  /// <param name="id">Group id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(string source)
  {
    if (uint.TryParse(source, out var id))
      return await GetDbContext().Groups.AnyAsync(x => x.Id == id);
    else
      return await GetDbContext().Groups.AnyAsync(x => x.Name == source);
  }

  public async Task DeleteAsync(string source)
  {
    var phys = await GetAsync(source);
    if (phys != null)
    {
      GetDbContext().Groups.Remove(phys);
      await GetDbContext().SaveChangesAsync();
    }
  }
}
