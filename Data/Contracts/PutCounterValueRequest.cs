using OLab.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLab.Data.Contracts;

public class PutCounterValueRequest
{
  public CountersDto Counter { get; set; }
  public DynamicScopedObjectsDto DynamicObjects { get; set; }
}
