using OLab.Api.Common;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule("QU")]
public class QuestionWikiTag : WikiTag1Argument
{
  public QuestionWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration, "OlabQuestionTag")
  {
  }

  public override string Translate(string source)
  {
    if (!HaveWikiTag(source))
      return source;

    while (HaveWikiTag(source))
    {
      var name = GetWikiArgument1();
      if (string.IsNullOrEmpty(name))
      {
        var str = $"**Invalid wiki tag {GetWiki()}**";
        Logger.LogDebug($"replacing {GetWiki()} <= {str}");
        source = ReplaceWikiTag(source, str);
      }
      else
      {
        var element = BuildWikiTagHTMLElement();
        Logger.LogDebug($"replacing {GetWiki()} <= {element}");
        source = ReplaceWikiTag(source, element);
      }
    }

    return source;
  }

}