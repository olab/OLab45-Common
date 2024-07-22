namespace OLab.Api.Dto;

public class FilesDto : ScopedObjectDto
{
  public string Url { get; set; }

  public FilesDto()
  {
    ObjectType = "file";
  }
}