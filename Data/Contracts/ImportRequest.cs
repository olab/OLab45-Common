using OLab.Utils;
using System.ComponentModel.DataAnnotations;

namespace OLab.Model
{
  public class ImportRequest
  {
    public ImportRequest()
    {
      MessageLevel = OLabLogMessage.MessageLevel.Info;
    }

    [Required]
    public string FileName { get; set; }
    public OLabLogMessage.MessageLevel MessageLevel { get; set; }
  }
}