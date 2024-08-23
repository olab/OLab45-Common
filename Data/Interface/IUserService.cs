using OLab.Api.Model;
using OLab.Data.Contracts;
using OLab.Data.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OLab.Data.Interface;

public interface IUserService
{
  //Users Authenticate(LoginRequest model);
  void ChangePassword(Users user, ChangePasswordRequest model);

  Task<List<AddUserResponse>> AddUsersAsync(List<AddUserRequest> items);  
  Task<List<AddUserResponse>> DeleteUsersAsync(List<AddUserRequest> items);
  Task<AddUserResponse> AddUserAsync(AddUserRequest item);
  Task<AddUserResponse> EditUserAsync(AddUserRequest item);
  Task<AddUserResponse> GetUsersAsync(AddUserRequest item);
  IList<UsersDto> GetUsers(string userName);


  IEnumerable<Users> GetAll();
  Users GetById(uint? id);
  Users GetByUserName(string userName);
}