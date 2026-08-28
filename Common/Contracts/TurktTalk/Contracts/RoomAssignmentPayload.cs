using OLab.Common.Contracts.TurktTalk.BusinessObjects;

namespace OLab.Common.Contracts.TurktTalk.Contracts;

public class RoomAssignmentPayload
{
  public Learner Local { get; set; }
  public Moderator Remote { get; set; }
  public int SlotIndex { get; set; }
}