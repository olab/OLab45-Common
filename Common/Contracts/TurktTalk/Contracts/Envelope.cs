using OLab.Common.Contracts.TurktTalk.BusinessObjects;

namespace OLab.Common.Contracts.TurktTalk.Contracts;

public class Envelope
{
  public string To { get; set; }
  public Learner From { get; set; }

  public Envelope()
  {
    From = new Learner();
  }
}
