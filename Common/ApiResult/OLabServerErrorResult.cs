using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OLabWebAPI.Common
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