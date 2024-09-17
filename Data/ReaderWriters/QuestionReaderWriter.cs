using Microsoft.EntityFrameworkCore;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class QuestionReaderWriter : ReaderWriter
{
  private readonly IOLabModuleProvider<IWikiTagModule> _wikiTagProvider;

  public static QuestionReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider)
  {
    return new QuestionReaderWriter(logger, context, wikiTagProvider);
  }

  public QuestionReaderWriter(
    IOLabLogger logger,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider) : base(logger, context)
  {
    _wikiTagProvider = wikiTagProvider;
  }

  /// <summary>
  /// Get question by id or name
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns>Roles</returns>
  public SystemQuestions Get(string source)
  {
    SystemQuestions phys;

    if (uint.TryParse(source, out var id))
      phys = GetDbContext().SystemQuestions.FirstOrDefault(x => x.Id == id);
    else
      phys = GetDbContext().SystemQuestions.FirstOrDefault(x => x.Name == source);

    return phys;
  }

  /// <summary>
  /// Get question by id or name
  /// </summary>
  /// <param name="id">Role Id</param>
  /// <returns>Roles</returns>
  public async Task<SystemQuestions> GetAsync(string source)
  {
    SystemQuestions phys;

    if (uint.TryParse(source, out var id))
      phys = await GetDbContext().SystemQuestions.FirstOrDefaultAsync(x => x.Id == id);
    else
      phys = await GetDbContext().SystemQuestions.FirstOrDefaultAsync(x => x.Name == source);

    return phys;
  }

  /// <summary>
  /// Disambiguate [[QU]] wikitags into specific question type
  /// Wikitags
  /// </summary>
  /// <param name="source">Source node text string</param>
  /// <returns>Revised node text string</returns>
  public string DisambiguateWikiQuestions(string source)
  {
    var wikiMatches = WikiTagUtils.GetWikiTags("QU", source);
    foreach (var wikiMatch in wikiMatches)
    {
      var idName = WikiTagUtils.GetWikiArgument1(wikiMatch);
      GetLogger().LogInformation($"disambiguating question '{wikiMatch}'");

      var questionPhys = Get(idName);
      if (questionPhys == null)
      {
        GetLogger().LogError($"unable to disambiguate question '{wikiMatch}'");
        continue;
      }

      var newWikiType = "QU";
      switch (questionPhys.EntryTypeId)
      {
        case 1:
          newWikiType = "QUST";
          break;
        case 2:
          newWikiType = "QUMT";
          break;
        case 3:
          newWikiType = "QUMP";
          break;
        case 4:
          newWikiType = "QUSP";
          break;
        case 5:
          newWikiType = "QUSD";
          break;
        case 6:
          newWikiType = "QUDG";
          break;
        case 12:
          newWikiType = "QUDP";
          break;
        default:
          break;
      }

      var newWikiTag = wikiMatch.Replace("QU:", $"{newWikiType}:");
      GetLogger().LogInformation($"disambiguating entry type {questionPhys.EntryTypeId}: '{wikiMatch}' => '{newWikiTag}'");

      source = source.Replace(wikiMatch, newWikiTag);
    }

    return source;
  }

  /// <summary>
  /// Get question(s) by scope level and scope id
  /// </summary>
  /// <param name="scopeLevel">Scope level</param>
  /// <param name="scopeId">Scope Id</param>
  /// <returns>Roles</returns>
  public async Task<IList<SystemQuestions>> GetAsync(string scopeLevel, uint id)
  {
    IList<SystemQuestions> phys = await GetDbContext().SystemQuestions.Where(x => x.ImageableType == scopeLevel && x.ImageableId == id).ToListAsync();
    return phys;
  }
}
