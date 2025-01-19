using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OLab.Api.Common;

public class OLabObjectResult<D> : ObjectResult
{
  public OLabObjectResult(object value, HttpStatusCode status = HttpStatusCode.OK) : base( value )
  {
  }

  public static OLabApiResult<D> Result(D value)
  {
    var result = new OLabApiResult<D>
    {
      Data = value,
      ErrorCode = System.Net.HttpStatusCode.OK
    };

    return result;
  }
}