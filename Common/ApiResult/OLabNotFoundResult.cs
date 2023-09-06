using Microsoft.AspNetCore.Mvc;

namespace OLab.Common
{
  public class OLabNotFoundResult<D>
  {
    public static NotFoundObjectResult Result(D value)
    {
      return new NotFoundObjectResult(new OLabAPIResponse<D>()
      {
        Data = value
      });
    }

    public static NotFoundObjectResult Result(string objectType, D value)
    {
      return new NotFoundObjectResult(new OLabAPIResponse<D>()
      {
        Message = $"{objectType}",
        Data = value
      });
    }
  }
}