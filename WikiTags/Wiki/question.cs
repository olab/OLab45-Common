using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Common.Utils;

[OLabModule("QU")]
public class QuestionWikiTag : WikiTag1ArgumentModule
{
  public QuestionWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    SetHtmlElementName("OlabQuestionTag");
  }

  public override string Translate(string source)
  {
    var wikiMatches = WikiTagUtils.GetWikiTags(GetWikiType(), source);

    if (wikiMatches.Count == 0)
      return source;

    foreach (var wikiMatch in wikiMatches)
    {
      wikiTagIdPart = WikiTagUtils.GetWikiArgument1(wikiMatch);
      if (string.IsNullOrEmpty(wikiTagIdPart))
      {
        var str = $"**Invalid wiki tag {GetWiki()}**";
        Logger.LogDebug($"replacing {GetWiki()} <= {str}");
        source = ReplaceWikiTag(source, str);
      }
      else
      {
        var element = BuildWikiTagHTMLElement();
        Logger.LogDebug($"replacing {wikiMatch} <= {element}");
        source = source.Replace(wikiMatch, element);
      }
    }

    return source;
  }

}