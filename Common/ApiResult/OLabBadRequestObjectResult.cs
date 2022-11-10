using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace OLabWebAPI.Common
{
  public class OLabBadRequestObjectResult
  {
    public static BadRequestObjectResult Result(string errorMessage = "Bad request")
    {
      return new BadRequestObjectResult(new OLabAPIResponse<string>()
      {
        Data = errorMessage,
        ErrorCode = HttpStatusCode.BadRequest
      });
    }

  }
}