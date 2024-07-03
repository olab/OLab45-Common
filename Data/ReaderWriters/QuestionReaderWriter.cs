using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using OLab.Api.Common;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLab.Data.ReaderWriters;

public class QuestionReaderWriter : ReaderWriter
{
  private WikiTagModuleProvider _tagProvider;

  public static QuestionReaderWriter Instance(
    IOLabLogger logger,
    OLabDBContext context, 
    WikiTagModuleProvider tagProvider)
  {
    return new QuestionReaderWriter(logger, context, tagProvider);
  }

  public QuestionReaderWriter(
    IOLabLogger logger,
    OLabDBContext context,
    WikiTagModuleProvider tagProvider) : base(logger, context)
  {
    _tagProvider = tagProvider;
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
    var questionTagModule = _tagProvider.GetModule("QU");

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
