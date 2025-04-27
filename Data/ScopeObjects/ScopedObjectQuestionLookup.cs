using Humanizer;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Data;

public partial class ScopedObjects
{
  private readonly IDictionary<uint, SystemQuestions> _questionIds = new Dictionary<uint, SystemQuestions>();
  private readonly IDictionary<string, SystemQuestions> _questionNames = new Dictionary<string, SystemQuestions>();
  
  public void AddQuestionCrossReference(SystemQuestions from, SystemQuestions to) 
  { 
    _questionIds.Add( from.Id, to );
    _questionNames.Add( from.Name, to ); 
  }

  public string GetQuestionCrossReference(string id)
  {
    if ( UInt32.TryParse( id, out uint QuestionId ) )
    {
      if ( _questionIds.ContainsKey( QuestionId ) )
        return _questionIds[ QuestionId ].Name;
    }
    else if ( _questionNames.ContainsKey( id ) )
      return _questionNames[ id ].Name;

    throw new KeyNotFoundException( $"Question '{id}' not found" );
  }

}
