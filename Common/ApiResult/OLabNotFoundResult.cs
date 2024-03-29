using System.Net;

namespace OLab.Api.Common;

public class OLabNotFoundResult<D>
{
  public static OLabAPIResponse<D> Result(D value)
  {
    return new OLabAPIResponse<D>()
    {
      Data = value,
      ErrorCode = HttpStatusCode.NotFound
    };
  }

  public static OLabAPIResponse<uint> Result(string objectType, uint value)
  {
    return new OLabAPIResponse<uint>()
    {
      Message = $"{objectType}",
      Data = value,
      ErrorCode = HttpStatusCode.NotFound
    };
  }
}