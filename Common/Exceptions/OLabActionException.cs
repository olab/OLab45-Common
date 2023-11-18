using Microsoft.AspNetCore.Mvc;
using System;

namespace OLab.Api.Common.Exceptions
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