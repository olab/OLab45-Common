using System.Net;

namespace OLab.Common.ApiResult;

public class OLabBadRequestObjectResult
{
  public static OLabApiResult<string> Result(string errorMessage = "Bad request")
  {
    return new OLabApiResult<string>
    {
      Data = errorMessage,
      ErrorCode = HttpStatusCode.BadRequest
    };
  }

}