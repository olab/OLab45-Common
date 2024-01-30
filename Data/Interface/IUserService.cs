using OLab.Api.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OLab.Data.Interface;

public interface IUserService
{
  Users Authenticate(LoginRequest model);
  void ChangePassword(Users user, ChangePasswordRequest model);

  Task<List<AddUserResponse>> AddUserAsync(List<AddUserRequest> items);
  Task<AddUserResponse> ProcessUserRequest(AddUserRequest item);

  IEnumerable<Users> GetAll();
  Users GetById(int id);
  Users GetByUserName(string userName);
}