using Microsoft.AspNetCore.Mvc;
using OLabWebAPI.Dto;

namespace OLabWebAPI.Data.Interface
{
    public interface IOLabAuthentication
    {
        IActionResult HasAccess(string acl, ScopedObjectDto dto);
        bool HasAccess(string acl, string objectType, uint? objectId);
        IUserContext GetUserContext();
    }
}