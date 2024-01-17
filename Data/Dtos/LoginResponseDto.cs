using OLab.Api.Contracts;

namespace OLab.Data.Dtos;

public class LoginResponseDto
{
  public LoginResponseDto()
  {
    AuthInfo = new RefreshToken();
  }

  public string UserName { get; set; }
  public string Group { get; set; }
  public string Role { get; set; }
  public RefreshToken AuthInfo { get; set; }
}
