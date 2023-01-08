using Microsoft.AspNetCore.Mvc;
using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.Player
{
  public partial class ReportEndpoint : OlabEndpoint
  {
    public ReportEndpoint(
      OLabLogger logger,
      OLabDBContext context) : base(logger, context)
    {
    }

    public async Task<ReportDataDto> GetAsync(string contextId)
    {
      var dto = new ReportDataDto();
      return dto;
    }
  }
}
