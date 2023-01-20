using System;
using System.Collections.Generic;

namespace OLabWebAPI.Dto
{
    public class ReportDataDto
    {
        public DateTime StartedAt { get; set; }
        public string Text { get; set; }

        public IList<ReportEntryDto> Entry { get; set; }
    }

    public class ReportEntryDto
    {
        public DateTime EntryAt { get; set; }
        public string Text { get; set; }
    }
}
