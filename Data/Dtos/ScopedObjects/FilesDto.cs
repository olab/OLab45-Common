using System;
using Newtonsoft.Json;

namespace OLabWebAPI.Dto
{
  public class FilesDto : ScopedObjectDto
  {
    public string Url { get; set; }
  }
}