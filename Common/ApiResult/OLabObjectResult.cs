using Microsoft.AspNetCore.Mvc;

namespace OLab.Common
{
  public class OLabObjectResult<D>
  {
    public static JsonResult Result(D value)
    {
      var result = new OLabAPIResponse<D>
      {
        Data = value,
        ErrorCode = System.Net.HttpStatusCode.OK
      };

      return new JsonResult(result);
    }
  }
}