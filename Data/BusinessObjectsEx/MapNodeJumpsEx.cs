using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using OLab.Common.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace OLab.Api.Model;

public partial class MapNodeJumps
{

  [NotMapped]
  public bool IsHidden
  {
    get => Hidden == 1;
    set => Hidden = value ? (sbyte)1 : (sbyte)0;
  }

}
