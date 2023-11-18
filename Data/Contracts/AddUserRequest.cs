using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OLab.Api.Model
{
  public class AddUserRequest
  {
    //private readonly string userRequestText;

    [Required]
    public string Username { get; set; }
    [Required]
    public string EMail { get; set; }
    [Required]
    public string NickName { get; set; }
    public string Password { get; set; }
    public string ModeUi { get; set; }
    public string Group { get; set; }
    [Required]
    public string Role { get; set; }

    public AddUserRequest()
    {
      NickName = "";
      ModeUi = "easy";
      Group = "";
      Role = "";
    }

    public AddUserRequest(string userRequestText)
    {
      var parts = userRequestText.Split("\t");
      if (parts.Length != 6)
      {
        throw new System.Exception("Bad user request record");
      }

      Username = parts[0];
      if (parts[1].Length > 0)
        Password = parts[1];
      EMail = parts[2];
      NickName = parts[3];
      Group = "";
      Role = parts[5];

      Username = Username.ToLower();

    }
  }
}