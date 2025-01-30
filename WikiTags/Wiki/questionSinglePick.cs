using OLab.Common.Attributes;
using OLab.Common.Interfaces;

[OLabModule( "QUSP" )]
public class QuestionSinglePickWikiTag : QuestionWikiTag
{
  public QuestionSinglePickWikiTag(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base( logger, configuration )
  {
    SetHtmlElementName( "OlabSinglePickQuestion" );
  }

}