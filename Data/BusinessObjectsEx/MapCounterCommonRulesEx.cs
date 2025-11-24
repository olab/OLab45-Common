using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class MapCounterCommonRules
{

  [NotMapped]
  public bool IsCorrect
  {
    get => Correct == 1;
    set => Correct = value ? (sbyte)1 : (sbyte)0;
  }

}
