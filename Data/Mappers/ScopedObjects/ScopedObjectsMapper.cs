using OLab.Api.Dto;
using OLab.Common.Interfaces;
using System;

namespace OLab.Api.ObjectMapper
{
  public class ScopedObjectsMapper : ObjectMapper<OLab.Data.BusinessObjects.ScopedObjects, ScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    //public ScopedObjectsMapper(IOLabLogger Logger, bool enableWikiTranslation = true) : base(Logger)
    //{
    //}

    public ScopedObjectsMapper(
      IOLabLogger logger,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
      bool enableWikiTranslation = true) : base(logger, wikiTagProvider)
    {
    }

    public override ScopedObjectsDto PhysicalToDto(
      OLab.Data.BusinessObjects.ScopedObjects phys,
      object source = null)
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

    public override OLab.Data.BusinessObjects.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
    {
      throw new NotImplementedException();
    }

  }
}