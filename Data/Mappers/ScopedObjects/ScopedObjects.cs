using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Utils;
using System;

namespace OLabWebAPI.ObjectMapper
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
      ScopedObjectsDto dto = GetDto(source);

      System.Collections.Generic.IList<QuestionsFullDto> dtoQuestionsList
        = new ObjectMapper.QuestionsFull(logger, GetWikiProvider()).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      System.Collections.Generic.IList<CountersDto> dtCountersList
        = new ObjectMapper.Counters(logger, GetWikiProvider()).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      System.Collections.Generic.IList<ConstantsDto> dtConstantsList
        = new ObjectMapper.Constants(logger, GetWikiProvider()).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      System.Collections.Generic.IList<FilesFullDto> dtFilesList
        = new ObjectMapper.FilesFull(logger, GetWikiProvider()).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      System.Collections.Generic.IList<ScriptsDto> dtScriptsList
        = new ObjectMapper.Scripts(logger, GetWikiProvider()).PhysicalToDto(phys.Scripts);
      dto.Scripts.AddRange(dtScriptsList);

      System.Collections.Generic.IList<ThemesFullDto> dtThemesList
        = new ThemesFull(logger, GetWikiProvider(), enableWikiTranslation).PhysicalToDto(phys.Themes);
      dto.Themes.AddRange(dtThemesList);

      System.Collections.Generic.IList<CounterActionsDto> dtCounterActionsList
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