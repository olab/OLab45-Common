using System.Collections.Generic;

namespace OLab.Api.Common;

public class OLabObjectPagedListResult<D>
{
  public static OLabAPIPagedResponse<D> Result(IList<D> value, int remaining)
  {
    var result = new OLabAPIPagedResponse<D>
    {
      Data = value,
      Remaining = remaining,
      Count = value.Count
    };

    return result;
  }
}