using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class MapQuestions
{

  [NotMapped]
  public bool IsShowAnswer
  {
    get => ShowAnswer == 1;
    set => ShowAnswer = value ? (sbyte)1 : (sbyte)0;
  }

  [NotMapped]
  public bool IsShowSubmit
  {
    get => ShowSubmit == 1;
    set => ShowSubmit = value ? (sbyte)1 : (sbyte)0;
  }

}
