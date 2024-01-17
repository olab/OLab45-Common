using OLab.Api.Utils;
using System.ComponentModel.DataAnnotations;

namespace OLab.Data.Contracts;

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