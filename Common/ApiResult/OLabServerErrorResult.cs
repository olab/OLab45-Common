using System.Net;

namespace OLab.Api.Common
{
  public class OLabServerErrorResult
  {
    public static OLabAPIResponse<string> Result(string errorMessage, HttpStatusCode ErrorCode = HttpStatusCode.InternalServerError)
    {
      return new OLabAPIResponse<string>()
      {
        Data = errorMessage,
        ErrorCode = ErrorCode,
        Message = "failed"
      };
    }

  }
}