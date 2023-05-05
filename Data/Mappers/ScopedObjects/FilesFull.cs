using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System.IO;

namespace OLabWebAPI.ObjectMapper
{
  public class FilesFull : OLabMapper<SystemFiles, FilesFullDto>
  {

    public FilesFull(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public FilesFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override FilesFullDto PhysicalToDto(SystemFiles phys, FilesFullDto source)
    {
      FilesFullDto dto = base.PhysicalToDto(phys, source);
      dto.FileName = Path.GetFileName(phys.Path);
      return dto;
    }

  }
}