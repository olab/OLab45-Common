using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "QUMT" )]
public class QuestionMultilineTextWikiTag : QuestionWikiTag
{
  public QuestionMultilineTextWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabMultilineTextQuestion" );
  }

}