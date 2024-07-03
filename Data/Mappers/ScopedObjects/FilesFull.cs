using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.IO;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class FilesFull : OLabMapper<SystemFiles, FilesFullDto>
{

  public FilesFull(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public FilesFull(IOLabLogger logger, WikiTagModuleProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override FilesFullDto PhysicalToDto(
    SystemFiles phys, 
    FilesFullDto source)
  {
    var dto = base.PhysicalToDto(phys, source);
    dto.FileName = Path.GetFileName(phys.Path);
    return dto;
  }

  public override SystemFiles DtoToPhysical(FilesFullDto dto)
  {
    var phys = base.DtoToPhysical(dto);
    phys.Path = dto.FileName;
    return phys;
  }
}