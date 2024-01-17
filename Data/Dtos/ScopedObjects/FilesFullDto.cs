using OLab.Common.Interfaces;
using System;
using System.IO;

namespace OLab.Data.Dtos;


public class FilesFullDto : FilesDto
{

  private readonly Stream FileContentsStream;

  public int? FileSize { get; set; }
  public int? Height { get; set; }
  public int? IsSystem { get; set; }
  public int? Type { get; set; }
  public int? Width { get; set; }
  public sbyte IsPrivate { get; set; }
  public sbyte IsShared { get; set; }
  public sbyte? IsEmbedded { get; set; }
  public string Args { get; set; }
  public string Copyright { get; set; }
  public string FileName { get; set; }
  public string HAlign { get; set; }
  public string HeightType { get; set; }
  public string Mime { get; set; }
  public string OriginUrl { get; set; }
  public string Path { get; set; }
  public string VAlign { get; set; }
  public string WidthType { get; set; }
  public string SelectedFileName { get; set; }
  public bool IsMediaResource { get; set; }

  public FilesFullDto()
  {
  }

  public override string ToString()
  {
    return $" '{Name}({Id})' = {FileName}";
  }

  public Stream GetStream() { return FileContentsStream; }

  public FilesFullDto(IOLabFormFieldHelper form)
  {
    Id = Convert.ToUInt32(form.Field("id"));
    Name = form.Field("name").ToString();
    Description = form.Field("description").ToString();
    Copyright = form.Field("copyright").ToString();
    ImageableId = Convert.ToUInt32(form.Field("parentId"));
    ImageableType = form.Field("scopeLevel").ToString();
    IsMediaResource = Convert.ToBoolean(form.Field("isMediaResource"));
    SelectedFileName = form.Fields["selectedFileName"].ToString();
    CreatedAt = DateTime.UtcNow;

    FileContentsStream = form.Stream;
    FileSize = Convert.ToInt32(FileContentsStream.Length);
    FileName = form.Fields["selectedFileName"].ToString();
  }

}