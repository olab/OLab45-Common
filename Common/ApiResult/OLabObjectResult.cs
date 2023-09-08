using Microsoft.AspNetCore.Mvc;

namespace OLab.Api.Common
{
  public class OLabObjectResult<D>
  {
  public static OLabAPIResponse<D> Result(D value)
  {
    var result = new OLabAPIResponse<D>
    {
      Data = value,
      ErrorCode = System.Net.HttpStatusCode.OK
    };

    return result;
  }
  }
}