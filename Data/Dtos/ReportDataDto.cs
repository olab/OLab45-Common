using OLabWebAPI.Model;
using System.Collections.Generic;
using System;

namespace OLabWebAPI.Dto
{
  public class ReportDataDto
  {
    public DateTime Timestamp { get; set; }
    public string Text { get; set; }

    public IList<ReportEntryDto> Entry { get; set; }
  }

  public class ReportEntryDto
  {
    public DateTime Timestamp { get; set; }
    public string Text{ get; set; }
  }
}
