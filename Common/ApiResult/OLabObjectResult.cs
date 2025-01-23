using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OLab.Api.Common;

public class OLabObjectResult<D> : ObjectResult
{
  public OLabObjectResult(object value, HttpStatusCode status = HttpStatusCode.OK) : base( value )
  {
  }

  public static OLabApiResult<D> Result(D value, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    var result = new OLabApiResult<D>
    {
      Data = value,
      ErrorCode = statusCode
    };

    return result;
  }
}