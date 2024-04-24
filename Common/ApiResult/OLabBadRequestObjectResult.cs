using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OLab.Api.Common;

public class OLabBadRequestObjectResult
{
  public static OLabAPIResponse<string> Result(string errorMessage = "Bad request")
  {
    return new OLabAPIResponse<string>
    {
      Data = errorMessage,
      ErrorCode = HttpStatusCode.BadRequest
    };
  }

}