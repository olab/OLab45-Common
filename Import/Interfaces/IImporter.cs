namespace OLab.Import.Interfaces;

public interface IImporter
{
  public bool LoadAll(string archiveFileName);
  public bool SaveAll();
}