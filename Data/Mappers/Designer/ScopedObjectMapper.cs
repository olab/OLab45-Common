using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Common.Interfaces;
using OLab.Data;
using System;
using System.Linq;

namespace OLab.Api.ObjectMapper.Designer;

public class ScopedObjectMapper : ObjectMapper<ScopedObjects, ScopedObjectsDto>
{
  protected readonly bool enableWikiTranslation = true;

  public ScopedObjectMapper(
    IOLabLogger logger,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    bool enableWikiTranslation = true) : base(logger, wikiTagProvider)
  {
  }

  public ScopedObjectMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override ScopedObjectsDto PhysicalToDto(
    ScopedObjects phys,
    object source = null)
  {
    var dto = GetDto(source);

    var dtConstantsList = new Constants(Logger).PhysicalToDto(phys.ConstantsPhys);
    dto.Constants.AddRange(dtConstantsList.OrderBy( x => x.Name));

    var dtoQuestionsList = new Questions(Logger).PhysicalToDto(phys.QuestionsPhys);
    dto.Questions.AddRange(dtoQuestionsList.OrderBy(x => x.Name));

    var dtCountersList = new Counters(Logger).PhysicalToDto(phys.CountersPhys);
    dto.Counters.AddRange(dtCountersList.OrderBy(x => x.Name));

    var dtFilesList = new Files(Logger).PhysicalToDto(phys.FilesPhys);
    dto.Files.AddRange(dtFilesList.OrderBy(x => x.Name));

    return dto;
  }

  public override ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
  {
    throw new NotImplementedException();
  }

}