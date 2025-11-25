using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class UserSessions
{

  [NotMapped]
  public bool IsNotCumulative
  {
    get => NotCumulative == 1;
    set => NotCumulative = value ? (sbyte)1 : (sbyte)0;
  }

}
