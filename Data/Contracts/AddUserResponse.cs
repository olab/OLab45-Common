namespace OLab.Api.Model;

public class AddUserResponse
{
  public uint Id { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Error { get; set; }

  public AddUserResponse()
  {
    Error = "OK";
  }

  public AddUserResponse(Users source)
  {
    Id = source.Id;
    Username = source.Username;
    Password = source.Password;
  }
}