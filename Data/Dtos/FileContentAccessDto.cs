using OLab.Data.BusinessObjectsEx.API;

namespace OLab.Api.Dto
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
