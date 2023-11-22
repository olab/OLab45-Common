using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace OLab.Api.Dto
{

  public class FilesFullDto : FilesDto
  {

    private Stream FileContentsStream = null;
    private IFormFile FormFile = null;

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
      return $" '{Name}({Id})' = {OriginUrl}";
    }

    public FilesFullDto(IFormCollection form)
    {
      Id = Convert.ToUInt32(form["id"]);
      Name = form["name"];
      Description = form["description"];
      Copyright = form["copyright"];
      ImageableId = Convert.ToUInt32(form["parentId"]);
      ImageableType = form["scopeLevel"];
      IsMediaResource = Convert.ToBoolean(form["isMediaResource"]);
      SelectedFileName = form["selectedFileName"];
      CreatedAt = DateTime.UtcNow;
      if (form.Files.Count == 0)
        throw new Exception("File not received");

      FileContentsStream = new MemoryStream();

      FormFile = form.Files[0]; // .CopyTo(FileContentsStream);
      FileSize = Convert.ToInt32( FormFile.Length );

      FileName = form["selectedFileName"];
    }

    public int GetFileSize()
    {
      if (FileContentsStream != null)
        return Convert.ToInt32( FileContentsStream.Length );
      else if (FormFile != null)
        return Convert.ToInt32( FormFile.Length );

      return 0;
    }

    public long GetFileContents(MemoryStream stream)
    {
      if ( FileContentsStream != null )
        FileContentsStream.CopyTo(stream);
      else if ( FormFile != null )
        FormFile.CopyTo(stream );

      stream.Position = 0;
      return stream.Length;
    }

    public FilesFullDto(MultipartFormDataParser parser)
    {
      Id = Convert.ToUInt32(parser.GetParameterValue("id"));
      Name = parser.GetParameterValue("name");
      Description = parser.GetParameterValue("description");
      Copyright = parser.GetParameterValue("copyright");
      ImageableId = Convert.ToUInt32(parser.GetParameterValue("parentId"));
      ImageableType = parser.GetParameterValue("scopeLevel");
      IsMediaResource = Convert.ToBoolean(parser.GetParameterValue("isMediaResource"));
      FileName = parser.GetParameterValue("selectedFileName");
      FileSize = Convert.ToInt32( parser.GetParameterValue("fileSize") );

      FileContentsStream = parser.Files[0].Data;
      FileContentsStream.Position = 0;
    }

  }

}