using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;
using System.IO;

namespace OLab.ObjectMapper
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