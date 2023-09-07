using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Utils;
using System;

namespace OLab.Api.ObjectMapper
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

    public override ScopedObjectsDto PhysicalToDto(Model.ScopedObjects phys, object source = null)
    {
      var dto = GetDto(source);

      var dtoQuestionsList
        = new ObjectMapper.QuestionsFull(logger, GetWikiProvider()).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList
        = new ObjectMapper.Counters(logger, GetWikiProvider()).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      var dtConstantsList
        = new ObjectMapper.Constants(logger, GetWikiProvider()).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      var dtFilesList
        = new ObjectMapper.FilesFull(logger, GetWikiProvider()).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      var dtScriptsList
        = new ObjectMapper.Scripts(logger, GetWikiProvider()).PhysicalToDto(phys.Scripts);
      dto.Scripts.AddRange(dtScriptsList);

      var dtThemesList
        = new ThemesFull(logger, GetWikiProvider(), enableWikiTranslation).PhysicalToDto(phys.Themes);
      dto.Themes.AddRange(dtThemesList);

      var dtCounterActionsList
        = new CounterActionsMapper(logger, GetWikiProvider()).PhysicalToDto(phys.CounterActions);
      dto.CounterActions.AddRange(dtCounterActionsList);

      return dto;
    }

    public override Model.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}