using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data;
using System;

namespace OLab.Api.ObjectMapper.Designer;

public class ScopedObjectMapper : ObjectMapper<ScopedObjects, ScopedObjectsDto>
{
  protected readonly bool enableWikiTranslation = true;

  public ScopedObjectMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  public override ScopedObjectsDto PhysicalToDto(
    ScopedObjects phys,
    object source = null)
  {
    var dto = GetDto(source);

    var dtConstantsList = new Constants(GetLogger(), GetDbContext(), GetWikiProvider()).PhysicalToDto(phys.ConstantsPhys);
    dto.Constants.AddRange(dtConstantsList);

    var dtoQuestionsList = new Questions(GetLogger(), GetDbContext(), GetWikiProvider()).PhysicalToDto(phys.QuestionsPhys);
    dto.Questions.AddRange(dtoQuestionsList);

    var dtCountersList = new Counters(GetLogger(), GetDbContext(), GetWikiProvider()).PhysicalToDto(phys.CountersPhys);
    dto.Counters.AddRange(dtCountersList);

    var dtFilesList = new Files(GetLogger(), GetDbContext(), GetWikiProvider()).PhysicalToDto(phys.FilesPhys);
    dto.Files.AddRange(dtFilesList);

    return dto;
  }

  public override ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
  {
    throw new NotImplementedException();
  }

}