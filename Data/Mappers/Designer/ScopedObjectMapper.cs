using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Designer;
using System;

namespace OLab.Data.Mappers.Designer
{
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

      var dtConstantsList = new ConstantsMapper(Logger).PhysicalToDto(phys.ConstantsPhys);
      dto.Constants.AddRange(dtConstantsList);

      var dtoQuestionsList = new Questions(Logger).PhysicalToDto(phys.QuestionsPhys);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList = new CountersMapper(Logger).PhysicalToDto(phys.CountersPhys);
      dto.Counters.AddRange(dtCountersList);

      var dtFilesList = new FilesMapper(Logger).PhysicalToDto(phys.FilesPhys);
      dto.Files.AddRange(dtFilesList);

      return dto;
    }

    public override ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}