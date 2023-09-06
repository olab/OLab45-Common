using Microsoft.AspNetCore.Mvc;
using OLab.Dto;

namespace OLab.Data.Interface
{
  public interface IOLabAuthentication
  {
    IActionResult HasAccess(string acl, ScopedObjectDto dto);
    bool HasAccess(string acl, string objectType, uint? objectId);
    IUserContext GetUserContext();
  }
}