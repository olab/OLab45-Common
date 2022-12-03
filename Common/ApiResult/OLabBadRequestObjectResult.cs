using Microsoft.AspNetCore.Mvc;
using System.Net;

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