using Microsoft.AspNetCore.Mvc;

namespace OLabWebAPI.Common
{
    public class OLabObjectResult<D>
    {
        public static JsonResult Result(D value)
        {
            var result = new OLabAPIResponse<D>
            {
                Data = value
            };

            return new JsonResult(result);
        }
    }
}