using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OLab.Common.Utils;

public class WikiTagUtils
{
  public static IList<string> GetTagNamePatterns()
  {
    return new string[] {
    "\"[.A-Za-z0-9\\- ]*\"",
    "[.A-Za-z0-9\\- ]*",
    "[0-9]*" };
  }

  public static IList<string> GetZeroArgumentTagPatterns(string wikiTag)
  {
    return new string[] {
      $"\\[\\[{wikiTag}\\]\\]"
    };
  }

  public static IList<string> GetOneArgumentTagPatterns(string wikiTag)
  {
    return new string[] {
      $"\\[\\[{wikiTag}:[0-9]*\\]\\]",
      $"\\[\\[{wikiTag}:\"[.A-Za-z0-9\\- ]*\"\\]\\]",
      $"\\[\\[{wikiTag}:[.A-Za-z0-9\\- ]*\\]\\]"
    };
  }

  /// <summary>
  /// Get wiki matches from string
  /// </summary>
  /// <param name="wikiTagPatterns">Regex tag patterns for match</param>
  /// <param name="source">source string</param>
  /// <returns>All matches</returns>
  public static IList<string> GetWikiTags(string wikiTag, string source)
  {
    var wikiTagPatterns = GetOneArgumentTagPatterns(wikiTag);

    var matches = new List<string>();

    foreach (var pattern in wikiTagPatterns)
    {
      foreach (var patternMatch in Regex.Matches(source, pattern))
        matches.Add(patternMatch.ToString());
    }

    return matches.Distinct().ToList();
  }

  public static string GetWikiArgument1(string wikiTag)
  {
    var source = wikiTag[(wikiTag.IndexOf(':') + 1)..].Replace("]]", "");
    foreach (var pattern in GetTagNamePatterns())
    {
      var regex = new Regex(pattern);
      var match = regex.Match(source);
      if (match.Success)
      {
        var wikiTagIdPart = match.Value.Replace("\"", "");
        return wikiTagIdPart;
      }
    }

    return null;
  }
}
