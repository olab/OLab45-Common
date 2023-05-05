using Microsoft.AspNetCore.Mvc;
using System;

namespace OLabWebAPI.Common.Exceptions
{
  public class OLabActionException : Exception
  {
    public IActionResult Result;

    public OLabActionException(IActionResult result)
    {
      Result = result;
    }
  }
}