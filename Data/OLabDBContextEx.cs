using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
