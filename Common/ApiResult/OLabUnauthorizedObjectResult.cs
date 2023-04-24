using Microsoft.AspNetCore.Mvc;
using OLabWebAPI.Common;
using System.Net;

namespace OLabWebAPI.Common
{
  public class OLabUnauthorizedObjectResult<D>
  {
    public static UnauthorizedObjectResult Result(D value)
    {
      return new UnauthorizedObjectResult(new OLabAPIResponse<D>()
      {
        Data = value,
        ErrorCode = HttpStatusCode.Unauthorized,
        Message = "Unauthorized"
      });
    }
  }
}
