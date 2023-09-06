using OLab.Api.Model;

namespace OLab.Api.Dto
{
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
}
