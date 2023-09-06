using OLab.Api.TurkTalk.BusinessObjects;

namespace OLab.Api.Common.Contracts
{
  public class RoomAssignmentPayload
  {
    public Learner Local { get; set; }
    public Moderator Remote { get; set; }
    public int SlotIndex { get; set; }
  }
}