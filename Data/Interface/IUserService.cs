using OLab.Api.Model;
using System.Collections.Generic;

namespace OLab.Data.Interface;

public interface IUserService
{
  Users Authenticate(LoginRequest model);
  void ChangePassword(Users user, ChangePasswordRequest model);

  void AddUser(Users newUser);
  IEnumerable<Users> GetAll();
  Users GetById(int id);
  Users GetByUserName(string userName);
}