using NuGet.Packaging;

using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Dtos;
using OLab.Data.Mappers;
using System;

namespace OLab.Data.Mappers
{
  public class ScopedObjectsMapper : ObjectMapper<ScopedObjects, ScopedObjectsDto>
  {
    private readonly bool _enableWikiTranslation = true;

    public ScopedObjectsMapper(
      IOLabLogger logger,
      IOLabModuleProvider<IWikiTagModule> wikiTagProvider = null,
      bool enableWikiTranslation = true) : base(logger, wikiTagProvider)
    {
      _enableWikiTranslation = enableWikiTranslation;
    }

    public override ScopedObjectsDto PhysicalToDto(
      ScopedObjects phys,
      object source = null)
    {
      var dto = GetDto(source);

      var dtoQuestionsList
        = new QuestionsFullMapper(Logger, _enableWikiTranslation).PhysicalToDto(phys.QuestionsPhys);
      dto.Questions.AddRange(dtoQuestionsList);

      var dtoCountersList
        = new CounterMapper(Logger, _enableWikiTranslation).PhysicalToDto(phys.CountersPhys);
      dto.Counters.AddRange(dtoCountersList);

      var dtoConstantsList
        = new ConstantsFullMapper(Logger, _enableWikiTranslation).PhysicalToDto(phys.ConstantsPhys);
      dto.Constants.AddRange(dtoConstantsList);

      var dtoFilesList
        = new FilesFullMapper(Logger, _enableWikiTranslation).PhysicalToDto(phys.FilesPhys);
      dto.Files.AddRange(dtoFilesList);

      var dtoScriptsList
        = new ScriptsMapper(Logger, _enableWikiTranslation).PhysicalToDto(phys.ScriptsPhys);
      dto.Scripts.AddRange(dtoScriptsList);

      if (_wikiTagModules != null)
      {
        var dtoThemesList
          = new ThemesFull(Logger, _wikiTagModules, _enableWikiTranslation).PhysicalToDto(phys.ThemesPhys);
        dto.Themes.AddRange(dtoThemesList);
      }

      var dtoCounterActionsList
        = new CounterActionsMapper(Logger).PhysicalToDto(phys.CounterActionsPhys);
      dto.CounterActions.AddRange(dtoCounterActionsList);

      return dto;
    }

    public override ScopedObjects DtoToPhysical(
      ScopedObjectsDto dto,
      object source = null)
    {
      var phys = new ScopedObjects();

      var physQuestions
        = new QuestionsFullMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.Questions);
      phys.QuestionsPhys.AddRange(physQuestions);

      var physCounters
        = new CounterMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.Counters);
      phys.CountersPhys.AddRange(physCounters);

      var physConstants
        = new ConstantsFullMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.Constants);
      phys.ConstantsPhys.AddRange(physConstants);

      var physFiles
        = new FilesFullMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.Files);
      phys.FilesPhys.AddRange(physFiles);

      var physScripts
        = new ScriptsMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.Scripts);
      phys.ScriptsPhys.AddRange(physScripts);

      if (_wikiTagModules != null)
      {
        var physThemes
          = new ThemesFull(Logger, _wikiTagModules, _enableWikiTranslation).DtoToPhysical(dto.Themes);
        phys.ThemesPhys.AddRange(physThemes);
      }

      var physActions
        = new CounterActionsMapper(Logger, _enableWikiTranslation).DtoToPhysical(dto.CounterActions);
      phys.CounterActionsPhys.AddRange(physActions);

      return phys;
    }

  }
}