using Microsoft.Extensions.Configuration;
using OLab.Api.Utils;
using OLab.Common.Interfaces;

namespace OLab.Api.Common
{
  public class WikiTagProvider : OLabModuleProvider<IWikiTagModule>
  {
    public WikiTagProvider(IOLabLogger logger, IConfiguration configuration) : base(logger, configuration)
    {
      Load("OLab.WikiTags*.dll");
    }

    /// <summary>
    /// Translate wiki tag into html
    /// </summary>
    /// <param name="source">Source text</param>
    /// <returns>Translated text</returns>
    public string Translate(string source)
    {
      // loop through all known wiki tag modules
      // asking them to apply an applicable text translation
      foreach (var module in Modules.Values)
        source = module.Translate(source);

      return source;
    }

  }
}
