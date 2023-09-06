using OLab.Model;

namespace OLab.Dto
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
