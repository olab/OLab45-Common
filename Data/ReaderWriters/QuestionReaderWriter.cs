using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OLab.Api.Dto.Designer;
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
    return new QuestionReaderWriter( logger, context, wikiTagProvider );
  }

  public QuestionReaderWriter(
  IOLabLogger logger,
  OLabDBContext context) : base( logger, context )
  {
  }

  public QuestionReaderWriter(
    IOLabLogger logger,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider) : base( logger, context )
  {
    _wikiTagProvider = wikiTagProvider;
  }


  /// <summary>
  /// Retrieves a SystemQuestions object based on the provided nodeId, mapId, and source.
  /// </summary>
  /// <param name="nodeId">The ID of the node.</param>
  /// <param name="mapId">The ID of the map.</param>
  /// <param name="source">The source string which can be either an ID or a name.</param>
  /// <returns>A SystemQuestions object if found; otherwise, null.</returns>
  public IList<SystemQuestions> Get()
  {
    return GetDbContext().SystemQuestions.ToList();
  }

  /// <summary>
  /// Retrieves a SystemQuestions object based on the provided nodeId, mapId, and source.
  /// </summary>
  /// <param name="nodeId">The ID of the node.</param>
  /// <param name="mapId">The ID of the map.</param>
  /// <param name="source">The source string which can be either an ID or a name.</param>
  /// <returns>A SystemQuestions object if found; otherwise, null.</returns>
  public SystemQuestions Get(uint nodeId, uint mapId, string source)
  {
    SystemQuestions phys = null;
    var questions = new List<SystemQuestions>();

    if ( uint.TryParse( source, out var id ) )
      questions = GetDbContext().SystemQuestions.Where( x => x.Id == id ).ToList();
    else
      questions = GetDbContext().SystemQuestions.Where( x => x.Name == source ).ToList();

    phys = questions.FirstOrDefault( x => x.ImageableType == Api.Utils.Constants.ScopeLevelNode && x.ImageableId == nodeId );
    if ( phys != null )
      return phys;

    phys = questions.FirstOrDefault( x => x.ImageableType == Api.Utils.Constants.ScopeLevelMap && x.ImageableId == mapId );
    if ( phys != null )
      return phys;

    phys = questions.FirstOrDefault( x => x.ImageableType == Api.Utils.Constants.ScopeLevelServer && x.ImageableId == 1 );
    if ( phys != null )
      return phys;

    return phys;
  }

  /// <summary>
  /// Disambiguates wiki questions based on the provided nodeId, mapId, and source.
  /// </summary>
  /// <param name="nodeId">The ID of the node.</param>
  /// <param name="mapId">The ID of the map.</param>
  /// <param name="source">The source string containing wiki questions to disambiguate.</param>
  /// <returns>A string with disambiguated wiki questions.</returns>
  public string DisambiguateWikiQuestions(uint nodeId, uint mapId, string source)
  {
    var wikiMatches = WikiTagUtils.GetWikiTags( "QU", source );
    foreach ( var wikiMatch in wikiMatches )
    {
      var idName = WikiTagUtils.GetWikiArgument1( wikiMatch );
      GetLogger().LogInformation( $"disambiguating question '{wikiMatch}'" );

      var questionPhys = Get( nodeId, mapId, idName );
      if ( questionPhys == null )
      {
        GetLogger().LogError( $"unable to disambiguate question '{wikiMatch}'" );
        continue;
      }

      var newWikiType = "QU";
      switch ( questionPhys.EntryTypeId )
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

      var newWikiTag = wikiMatch.Replace( "QU:", $"{newWikiType}:" );
      GetLogger().LogInformation( $"disambiguating entry type {questionPhys.EntryTypeId}: '{wikiMatch}' => '{newWikiTag}'" );

      source = source.Replace( wikiMatch, newWikiTag );
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
    IList<SystemQuestions> phys = await GetDbContext().SystemQuestions.Where( x => x.ImageableType == scopeLevel && x.ImageableId == id ).ToListAsync();
    return phys;
  }
}
