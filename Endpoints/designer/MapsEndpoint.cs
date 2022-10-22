using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLabWebAPI.Model;
using OLabWebAPI.Dto;
using OLabWebAPI.ObjectMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Interface;
using OLabWebAPI.Utils;
using System.Text;

namespace OLabWebAPI.Endpoints.Designer
{
  public partial class MapsEndpoint : OlabEndpoint
  {

    public MapsEndpoint( 
      OLabLogger logger, 
      OLabDBContext context, 
      IOlabAuthentication auth) : base(logger, context, auth)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Model.Maps GetSimple(OLabDBContext context, uint id)
    {
      var phys = context.Maps.Include(x => x.SystemCounterActions).FirstOrDefault(x => x.Id == id);
      return phys;
    }

  }
}
