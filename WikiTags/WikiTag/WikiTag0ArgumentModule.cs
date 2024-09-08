using OLab.Common.Interfaces;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OLab.Api.Common;

public abstract class WikiTag0ArgumentModule : WikiTagModule
{
  public WikiTag0ArgumentModule(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
  }

  public override void SetHtmlElementName(string elementName)
  {
    base.SetHtmlElementName(elementName);
    wikiTagPatterns = WikiTagUtils.GetZeroArgumentTagPatterns(GetWikiType()).ToList();
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