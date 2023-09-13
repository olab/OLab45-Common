using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using System;

namespace OLab.Api.ObjectMapper
{
  public class ScopedObjects : ObjectMapper<Model.ScopedObjects, ScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    //public ScopedObjects(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    //{
    //}

    public ScopedObjects(
      IOLabLogger logger, 
      IOLabModuleProvider<IWikiTagModule> wikiTagModules, 
      bool enableWikiTranslation = true) : base(logger, wikiTagModules)
    {
    }

    public override ScopedObjectsDto PhysicalToDto(Model.ScopedObjects phys, object source = null)
    {
      var dto = GetDto(source);

      var dtoQuestionsList
        = new ObjectMapper.QuestionsFull(Logger).PhysicalToDto(phys.Questions);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtCountersList
        = new ObjectMapper.Counters(Logger).PhysicalToDto(phys.Counters);
      dto.Counters.AddRange(dtCountersList);

      var dtConstantsList
        = new ObjectMapper.Constants(Logger).PhysicalToDto(phys.Constants);
      dto.Constants.AddRange(dtConstantsList);

      var dtFilesList
        = new ObjectMapper.FilesFull(Logger).PhysicalToDto(phys.Files);
      dto.Files.AddRange(dtFilesList);

      var dtScriptsList
        = new ObjectMapper.Scripts(Logger).PhysicalToDto(phys.Scripts);
      dto.Scripts.AddRange(dtScriptsList);

      var dtThemesList
        = new ThemesFull(Logger, _wikiTagModules).PhysicalToDto(phys.Themes);
      dto.Themes.AddRange(dtThemesList);

      var dtCounterActionsList
        = new CounterActionsMapper(Logger).PhysicalToDto(phys.CounterActions);
      dto.CounterActions.AddRange(dtCounterActionsList);

      return dto;
    }

    public override Model.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}