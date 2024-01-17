using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using System.IO;

namespace OLab.Data.Mappers;

public class FilesFullMapper : OLabMapper<SystemFiles, FilesFullDto>
{

  public FilesFullMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public FilesFullMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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