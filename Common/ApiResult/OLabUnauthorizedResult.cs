using Microsoft.AspNetCore.Mvc;

namespace OLab.Common
{
  public class OLabUnauthorizedResult
  {
    public static UnauthorizedResult Result()
    {
      return new UnauthorizedResult();
    }
  }
}