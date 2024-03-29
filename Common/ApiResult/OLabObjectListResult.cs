using System.Collections.Generic;

namespace OLab.Api.Common;

public class OLabObjectListResult<D>
{
  public static OLabAPIResponse<IList<D>> Result(IList<D> value)
  {
    var result = new OLabAPIPagedResponse<D>
    {
      Data = value,
      Count = value.Count
    };

    return result;
  }
}