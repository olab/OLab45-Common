namespace OLab.Api.Model;

public class AddUserResponse
{
  public string Username { get; set; }
  public string Password { get; set; }
  public string Message { get; set; }

  public AddUserResponse()
  {
    Message = "OK";
  }
}