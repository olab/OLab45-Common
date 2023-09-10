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
        = new ObjectMapper.QuestionsFull(Logger, GetWikiProvider()).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList
        = new ObjectMapper.Counters(Logger, GetWikiProvider()).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      var dtConstantsList
        = new ObjectMapper.Constants(Logger, GetWikiProvider()).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      var dtFilesList
        = new ObjectMapper.FilesFull(Logger, GetWikiProvider()).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      var dtScriptsList
        = new ObjectMapper.Scripts(Logger, GetWikiProvider()).PhysicalToDto(phys.Scripts);
      dto.Scripts.AddRange(dtScriptsList);

      var dtThemesList
        = new ThemesFull(Logger, GetWikiProvider(), enableWikiTranslation).PhysicalToDto(phys.Themes);
      dto.Themes.AddRange(dtThemesList);

      var dtCounterActionsList
        = new CounterActionsMapper(Logger, GetWikiProvider()).PhysicalToDto(phys.CounterActions);
      dto.CounterActions.AddRange(dtCounterActionsList);

      return dto;
    }

    public override Model.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}