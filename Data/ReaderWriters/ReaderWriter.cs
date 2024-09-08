using OLab.Api.Model;
using OLab.Common.Interfaces;

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

}
