using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "QUST" )]
public class QuestionSinglelineTextWikiTag : QuestionWikiTag
{
  public QuestionSinglelineTextWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabSinglelineTextQuestion" );
  }

}