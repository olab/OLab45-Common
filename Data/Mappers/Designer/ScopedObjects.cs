using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Common.Interfaces;
using System;

namespace OLab.Api.ObjectMapper.Designer
{
  public class ScopedObjects : ObjectMapper<OLab.Data.BusinessObjects.ScopedObjects, ScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    public ScopedObjects(
      IOLabLogger logger,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
      bool enableWikiTranslation = true) : base(logger, wikiTagProvider)
    {
    }

    public ScopedObjects(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override ScopedObjectsDto PhysicalToDto(
      OLab.Data.BusinessObjects.ScopedObjects phys,
      object source = null)
    {
      var dto = GetDto(source);

      var dtConstantsList = new Constants(Logger).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      var dtoQuestionsList = new Questions(Logger).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList = new Counters(Logger).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      var dtFilesList = new Files(Logger).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      return dto;
    }

    public override OLab.Data.BusinessObjects.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}