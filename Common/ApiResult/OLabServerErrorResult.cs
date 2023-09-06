using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OLab.Common
{
  public class OLabServerErrorResult
  {
    public static BadRequestObjectResult Result(string errorMessage, HttpStatusCode ErrorCode = HttpStatusCode.InternalServerError)
    {
      return new BadRequestObjectResult(new OLabAPIResponse<string>()
      {
        Data = errorMessage,
        ErrorCode = ErrorCode,
        Message = "failed"
      });
    }

  }
}