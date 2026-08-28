using System.Collections.Generic;

namespace OLab.Common.ApiResult;

public class OLabObjectListResult<D>
{
  public static OLabApiResult<IList<D>> Result(IList<D> value)
  {
    var result = new OLabAPIPagedResponse<D>
    {
      Data = value,
      Count = value.Count
    };

    return result;
  }
}