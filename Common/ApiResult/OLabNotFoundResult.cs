using System.Net;

namespace OLab.Api.Common;

public class OLabNotFoundResult<D>
{
  public static OLabApiResult<D> Result(D value)
  {
    return new OLabApiResult<D>()
    {
      Data = value,
      ErrorCode = HttpStatusCode.NotFound
    };
  }

  public static OLabApiResult<uint> Result(string objectType, uint value)
  {
    return new OLabApiResult<uint>()
    {
      Message = $"{objectType}",
      Data = value,
      ErrorCode = HttpStatusCode.NotFound
    };
  }
}