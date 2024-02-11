
using OLab.Api.TurkTalk.BusinessObjects;

namespace OLab.Api.Common.Contracts;

public class Envelope
{
  public string To { get; set; }
  public Learner From { get; set; }

  public Envelope()
  {
    From = new Learner();
  }
}
