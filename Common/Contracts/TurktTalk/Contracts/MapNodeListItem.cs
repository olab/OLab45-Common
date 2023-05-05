namespace OLabWebAPI.Common.Contracts
{
  public class MapNodeListItem
  {
    public uint Id { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
      return $"{Name}({Id})";
    }
  }
}
