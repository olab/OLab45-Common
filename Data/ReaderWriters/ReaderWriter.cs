using Microsoft.Build.Framework;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Data.ReaderWriters;

public abstract class ReaderWriter
{
  protected readonly OLabDBContext _context;
  protected readonly IOLabLogger _logger;

  public ReaderWriter(IOLabLogger logger, OLabDBContext context)
  {
    _context = context;
    _logger = logger;
  }

}
