﻿using OLab.Api.Model;
using System.Linq;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OLab.Api.Common;

namespace OLab.Data.ReaderWriters;

public class RoleReaderWriter : ReaderWriter
{
  public static RoleReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context)
  {
    return new RoleReaderWriter(logger, context);
  }

  public RoleReaderWriter(
    IOLabLogger logger,
    OLabDBContext context) : base(logger, context)
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
    var existingPhys = await GetAsync(name);

    if (existingPhys == null)
    {
      _logger.LogInformation($"creating grpup '{newPhys.Name}'");

      _context.Roles.Add(newPhys);
      _context.SaveChanges();
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

    if (uint.TryParse(source, out uint id))
      phys = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
    else
      phys = await _context.Roles.FirstOrDefaultAsync(x => x.Name == source);

    return phys;
  }

  /// <summary>
  /// Get groups paged
  /// </summary>
  /// <param name="take">(optional) number of objects to return</param>
  /// <param name="skip">(optional) number of objects to skip</param>
  /// <returns>OLabAPIPagedResponse</returns>
  public async Task<OLabAPIPagedResponse<Roles>> GetPagedAsync(int? take, int? skip)
  {
    var response = new OLabAPIPagedResponse<Roles>();

    if (!take.HasValue && !skip.HasValue)
    {
      response.Data = await _context.Roles.ToListAsync();
      response.Count = response.Data.Count;
      response.Remaining = 0;
    }

    else if (take.HasValue && skip.HasValue)
    {
      response.Data = await _context.Roles.Skip(skip.Value).Take(take.Value).ToListAsync();
      response.Count += response.Data.Count;
      response.Remaining = _context.Roles.Count() - skip.Value - response.Count;
    }

    else
      _logger.LogWarning($"invalid/partial take/skip parameters");

    return response;
  }

  /// <summary>
  /// Test if group exists by id
  /// </summary>
  /// <param name="id">Role id</param>
  /// <returns>true/false</returns>
  public async Task<bool> ExistsAsync(string source)
  {
    if (uint.TryParse(source, out uint id))
      return await _context.Roles.AnyAsync(x => x.Id == id);
    else
      return await _context.Roles.AnyAsync(x => x.Name == source);
  }

  public async Task DeleteAsync(string source)
  {
    var phys = await GetAsync(source);
    if (phys != null)
    {
      _context.Roles.Remove(phys);
      await _context.SaveChangesAsync();
    }
  }
}
