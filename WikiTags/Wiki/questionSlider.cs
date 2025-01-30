using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "QUSD" )]
public class QuestionSliderWikiTag : QuestionWikiTag
{
  public QuestionSliderWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabSliderQuestion" );
  }

}