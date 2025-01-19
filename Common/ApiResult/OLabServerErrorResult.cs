using System.Net;

namespace OLab.Api.Common;

public class OLabServerErrorResult
{
  public static OLabApiResult<string> Result(string errorMessage, HttpStatusCode ErrorCode = HttpStatusCode.InternalServerError)
  {
    return new OLabApiResult<string>()
    {
      Data = errorMessage,
      ErrorCode = ErrorCode,
      Message = "failed"
    };
  }

}