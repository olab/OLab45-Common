using System.Net;

namespace OLab.Api.Common;

public class OLabObjectResult<D>
{
  public static OLabAPIResponse<D> Result(D value, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    var result = new OLabAPIResponse<D>
    {
      Data = value,
      ErrorCode = statusCode
    };

    if (statusCode != HttpStatusCode.OK)
      result.Message = statusCode.ToString();

    return result;
  }
}