using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System.IO;

namespace OLab.Api.ObjectMapper;

public class FilesFull : OLabMapper<SystemFiles, FilesFullDto>
{

  public FilesFull(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }

  public FilesFull(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }

  public override FilesFullDto PhysicalToDto(
    SystemFiles phys,
    FilesFullDto source)
  {
    var dto = base.PhysicalToDto( phys, source );
    dto.FileName = Path.GetFileName( phys.Path );
    return dto;
  }

  public override SystemFiles DtoToPhysical(FilesFullDto dto)
  {
    var phys = base.DtoToPhysical( dto );
    phys.Path = dto.FileName;
    return phys;
  }
}