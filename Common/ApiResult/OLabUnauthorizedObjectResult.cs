using System.Net;

namespace OLab.Api.Common;

public class OLabUnauthorizedObjectResult
{
  public static OLabAPIResponse<string> Result(string value)
  {
    var apiResponse = new OLabAPIResponse<string>()
    {
      Data = value,
      ErrorCode = HttpStatusCode.Unauthorized,
      Message = "Unauthorized"
    };

    return apiResponse;
  }
}
