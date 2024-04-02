using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OLab.Api.Common;

public abstract class WikiTag0Argument : WikiTagModule
{
  public WikiTag0Argument(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    string htmlElementName) : base(logger, configuration, htmlElementName)
  {
    wikiTagPatterns.Add($"\\[\\[{GetWikiType()}\\]\\]");
  }

  protected override string BuildWikiTagHTMLElement()
  {
    var doc = new XDocument();
    var xml = new XElement(
      GetHtmlElementName(),
      string.Empty
    );

    var classes = new List<string> { GetHtmlElementName() };
    xml.SetAttributeValue("class", string.Join(" ", classes));
    xml.SetAttributeValue("props", "{props}");

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
      var element = BuildWikiTagHTMLElement();
      Logger.LogDebug($"replacing {GetWiki()} <= {element}");
      source = ReplaceWikiTag(source, element);
    }

    return source;
  }

}