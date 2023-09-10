using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Api.Utils;
using System;

namespace OLab.Api.ObjectMapper.Designer
{
  public class ScopedObjects : ObjectMapper<Model.ScopedObjects, ScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    public ScopedObjects(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public ScopedObjects(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override ScopedObjectsDto PhysicalToDto(
      Model.ScopedObjects phys,
      object source = null)
    {
      var dto = GetDto(source);

      var dtConstantsList = new Constants(Logger, GetWikiProvider()).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      var dtoQuestionsList = new Questions(Logger, GetWikiProvider()).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList = new Counters(Logger, GetWikiProvider()).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      var dtFilesList = new Files(Logger, GetWikiProvider()).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      return dto;
    }

    public override Model.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}