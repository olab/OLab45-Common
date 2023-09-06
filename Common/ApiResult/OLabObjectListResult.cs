using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OLab.Common
{
  public class OLabObjectListResult<D>
  {
    public static JsonResult Result(IList<D> value)
    {
      var result = new OLabAPIPagedResponse<D>
      {
        Data = value,
        Count = value.Count
      };

      return new JsonResult(result);
    }
  }
}