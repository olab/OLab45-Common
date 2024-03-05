using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OLab.Api.Common;

public abstract class WikiTag1Argument : WikiTagModule
{
  protected string wikiTagIdPart;
  protected List<string> wikiTagNamePatterns = new();

  public WikiTag1Argument(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    string htmlElementName) : base(logger, configuration, htmlElementName)
  {
    wikiTagPatterns.Add($"\\[\\[{GetWikiType()}:[0-9]*\\]\\]");
    wikiTagPatterns.Add($"\\[\\[{GetWikiType()}:\"[.A-Za-z0-9\\- ]*\"\\]\\]");
    wikiTagPatterns.Add($"\\[\\[{GetWikiType()}:[.A-Za-z0-9\\- ]*\\]\\]");
    wikiTagNamePatterns.Add("\"[.A-Za-z0-9\\- ]*\"");
    wikiTagNamePatterns.Add("[.A-Za-z0-9\\- ]*");
    wikiTagNamePatterns.Add("[0-9]*");
  }

  public string Set(string wikiType, string wikiId)
  {
    SetWikiType(wikiType);
    SetWikiId(wikiId);
    return _wiki;
  }

  public string GetWikiId() { return wikiTagIdPart; }
  public void SetWikiId(string wikiTag)
  {
    wikiTagIdPart = wikiTag;
    _wiki = $"[[{GetWikiType()}:{GetWikiId()}]]";
  }

  public override void SetWikiType(string wikiTag)
  {
    _wikiType = wikiTag;
    _wiki = $"[[{GetWikiType()}:{GetWikiId()}]]";
  }

  public string GetWikiArgument1()
  {
    var source = GetWiki()[(GetWiki().IndexOf(':') + 1)..].Replace("]]", "");
    foreach (var pattern in wikiTagNamePatterns)
    {
      var regex = new Regex(pattern);
      var match = regex.Match(source);
      if (match.Success)
      {
        wikiTagIdPart = match.Value.Replace("\"", "");
        return wikiTagIdPart;
      }
    }

    return null;
  }

  public override bool HaveWikiTag(string source)
  {
    if (!base.HaveWikiTag(source)) return false;

    // extract/set argument1
    GetWikiArgument1();

    return true;
  }

  public string PreviewNewArgument1(string argument)
  {
    return $"[[{GetWikiType()}:{argument}]]";
  }

  protected override string BuildWikiTagHTMLElement()
  {
    var doc = new XDocument();
    var xml = new XElement(
      GetHtmlElementName(),
      string.Empty
    );

    var classes = new List<string> { GetHtmlElementName().ToLower(), wikiTagIdPart.ToLower().Replace(" ", "-") };
    xml.SetAttributeValue("class", string.Join(" ", classes));
    xml.SetAttributeValue("props", "{props}");
    xml.SetAttributeValue("name", $"{wikiTagIdPart}");

    doc.Add(xml);

    // de-quote any attributes which are bindings
    var element = doc.ToString();
    element = element.Replace("\"{", "{");
    element = element.Replace("}\"", "}");

    return element;
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