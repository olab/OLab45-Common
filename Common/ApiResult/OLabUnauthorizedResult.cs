using Microsoft.AspNetCore.Mvc;

namespace OLab.Api.Common;

public class OLabUnauthorizedResult
{
  public static UnauthorizedResult Result()
  {
    return new UnauthorizedResult();
  }
}