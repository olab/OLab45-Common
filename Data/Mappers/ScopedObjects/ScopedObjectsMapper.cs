using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data;

namespace OLab.Api.ObjectMapper;

public class ScopedObjectsMapper : ObjectMapper<ScopedObjects, ScopedObjectsDto>
{
  private readonly bool _enableWikiTranslation = true;

  public ScopedObjectsMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
    _enableWikiTranslation = enableWikiTranslation;
  }

  public override ScopedObjectsDto PhysicalToDto(
    ScopedObjects phys,
    object source = null)
  {
    var dto = GetDto(source);

    var dtoQuestionsList
      = new QuestionsFullMapper(
        GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.QuestionsPhys);
    dto.Questions.AddRange(dtoQuestionsList);

    var dtoCountersList
      = new CounterMapper(GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.CountersPhys);
    dto.Counters.AddRange(dtoCountersList);

    var dtoConstantsList
      = new ConstantsFull(GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.ConstantsPhys);
    dto.Constants.AddRange(dtoConstantsList);

    var dtoFilesList
      = new FilesFull(GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.FilesPhys);
    dto.Files.AddRange(dtoFilesList);

    var dtoScriptsList
      = new ScriptsFull(GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.ScriptsPhys);
    dto.Scripts.AddRange(dtoScriptsList);

    if (_wikiTagModules != null)
    {
      var dtoThemesList
        = new ThemesFull(
          GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.ThemesPhys);
      dto.Themes.AddRange(dtoThemesList);
    }

    var dtoCounterActionsList
      = new CounterActionsMapper(GetLogger(),
        GetDbContext(),
        GetWikiProvider(),
        _enableWikiTranslation).PhysicalToDto(phys.CounterActionsPhys);
    dto.CounterActions.AddRange(dtoCounterActionsList);

    return dto;
  }

  public override ScopedObjects DtoToPhysical(
    ScopedObjectsDto dto,
    object source = null)
  {
    var phys = new ScopedObjects();

    var physQuestions
      = new QuestionsFullMapper(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Questions);
    phys.QuestionsPhys.AddRange(physQuestions);

    var physCounters
      = new CounterMapper(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Counters);
    phys.CountersPhys.AddRange(physCounters);

    var physConstants
      = new ConstantsFull(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Constants);
    phys.ConstantsPhys.AddRange(physConstants);

    var physFiles
      = new FilesFull(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Files);
    phys.FilesPhys.AddRange(physFiles);

    var physScripts
      = new ScriptsFull(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Scripts);
    phys.ScriptsPhys.AddRange(physScripts);

    if (_wikiTagModules != null)
    {
      var physThemes
        = new ThemesFull(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.Themes);
      phys.ThemesPhys.AddRange(physThemes);
    }

    var physActions
      = new CounterActionsMapper(GetLogger(), GetDbContext(), GetWikiProvider(), _enableWikiTranslation).DtoToPhysical(dto.CounterActions);
    phys.CounterActionsPhys.AddRange(physActions);

    return phys;
  }

}