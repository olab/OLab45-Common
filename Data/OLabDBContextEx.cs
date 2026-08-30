using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

public partial class OLabDBContext : DbContext
{
  public OLabDBContext()
  {

  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {

  }

}
