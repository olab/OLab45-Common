using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.IO;

namespace OLab.Api.ObjectMapper;

public class FilesFull : OLabMapper<SystemFiles, FilesFullDto>
{

  public FilesFull(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public FilesFull(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override FilesFullDto PhysicalToDto(SystemFiles phys, FilesFullDto source)
  {
    var dto = base.PhysicalToDto(phys, source);
    dto.FileName = Path.GetFileName(phys.Path);
    return dto;
  }

  public override SystemFiles DtoToPhysical(FilesFullDto source)
  {
    var phys = base.DtoToPhysical(source);
    phys.Path = source.FileName;
    return phys;
  }
}