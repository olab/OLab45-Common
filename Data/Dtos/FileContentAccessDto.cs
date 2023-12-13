using OLab.Api.Contracts;
using OLab.Data.Models;

namespace OLab.Data.Dtos
{
  public class FileContentAccessDto
  {
    public FileContentAccessDto()
    {
      AuthInfo = new RefreshToken();
    }

    public uint FileId { get; set; }
    public RefreshToken AuthInfo { get; set; }
  }
}
