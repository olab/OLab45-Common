using Microsoft.AspNetCore.Mvc;
using OLab.Api.Dto;

namespace OLab.Api.Data.Interface
{
  public interface IOLabAuthentication
  {
    IActionResult HasAccess(string acl, ScopedObjectDto dto);
    bool HasAccess(string acl, string objectType, uint? objectId);
    IUserContext GetUserContext();
  }
}