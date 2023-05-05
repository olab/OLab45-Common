using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OLabWebAPI.Common
{
  public class OLabObjectPagedListResult<D>
  {
    public static JsonResult Result(IList<D> value, int remaining)
    {
      var result = new OLabAPIPagedResponse<D>
      {
        Data = value,
        Remaining = remaining,
        Count = value.Count
      };

      return new JsonResult(result);
    }
  }
}