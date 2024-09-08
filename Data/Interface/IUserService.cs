using OLab.Api.Model;
using OLab.Data.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OLab.Data.Interface;

public interface IUserService
{
  //Users Authenticate(LoginRequest model);
  void ChangePassword(Users user, ChangePasswordRequest model);

  Task<List<UsersDto>> AddUsersAsync(List<AddUserRequest> items);  
  Task<List<AddUserResponse>> DeleteUsersAsync(List<DeleteUsersRequest> items);
  Task<UsersDto> AddUserAsync(AddUserRequest item);
  Task<UsersDto> EditUserAsync(AddUserRequest item);
  IList<UsersDto> GetUsers(string userName);

  Users GetById(uint? id);
  Users GetByUserName(string userName);
  Task<List<UsersDto>> ImportUsersAsync(Stream fileStream);
}