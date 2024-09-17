using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OLab.Api.Common;

public abstract class WikiTagModule : IWikiTagModule
{
  protected List<string> wikiTagPatterns = new();
  protected string _wikiType;
  protected string _wiki;
  private string _htmlElementName;
  protected readonly IOLabConfiguration _configuration;

  protected int wikiStart = 0;
  protected int wikiEnd = 0;
  protected IOLabLogger Logger;

  public WikiTagModule(
    IOLabLogger logger,
    IOLabConfiguration configuration)
  {
    var t = GetType();
    var attribute =
        (OLabModuleAttribute)Attribute.GetCustomAttribute(t, typeof(OLabModuleAttribute));
    _wikiType = attribute.Name;

    Logger = logger;
    _configuration = configuration;
  }

  public string GetWikiType() { return _wikiType; }
  public virtual void SetWikiType(string wikiTag) { _wikiType = wikiTag; }
  public string GetHtmlElementName() { return _htmlElementName; }
  public abstract string Translate(string source);
  protected abstract string BuildWikiTagHTMLElement();

  public virtual void SetHtmlElementName(string elementName)
  {
    _htmlElementName = elementName;
  }

  public string GetUnquotedWiki()
  {
    var wiki = GetWiki().Replace("[[", "[");
    wiki = wiki.Replace("]]", "]");
    return wiki;
  }

  public string GetWiki() { return _wiki; }
  public string BuildWiki(uint id, string name = "")
  {
    if (string.IsNullOrEmpty(name))
      return $"[[{GetWikiType()}:{id}]]";
    return $"[[{GetWikiType()}:{name}]]";
  }

  protected string ReplaceWikiTag(string original, string replaceString)
  {
    var preWikiPart = original[..wikiStart];
    var postWikiPart = original[wikiEnd..];
    return preWikiPart + replaceString + postWikiPart;
  }

  /// <summary>
  /// Test if have wiki tag in source string
  /// </summary>
  /// <param name="source">Source text</param>
  /// <returns>true/false</returns>
  public virtual bool HaveWikiTag(string source)
  {
    foreach (var pattern in wikiTagPatterns)
    {
      var regex = new Regex(pattern);
      var match = regex.Match(source);
      if (match.Success)
      {
        wikiStart = match.Index;
        wikiEnd = match.Index + match.Length;
        _wiki = match.Value;
        return true;
      }
    }

    return false;
  }
}