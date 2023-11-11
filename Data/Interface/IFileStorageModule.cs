using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data.Interface
{
  public interface IFileStorageModule
  {
    string GetModuleName();

    char GetFolderSeparator();

    void AttachUrls(
      IList<SystemFiles> items);

    public Task MoveFileAsync(
      string fileName,
      string sourcePath,
      string destinationPath,
      CancellationToken token = default);

    Task<string> UploadImportFileAsync(
      Stream stream,
      string fileName,
      CancellationToken token);

    Task<string> UploadMapFileAsync(
      Stream stream,
      FilesFullDto dto,
      CancellationToken token);

    bool FileExists(
      string folder,
      string fileName);

    Task<Stream> ReadFileAsync(
      string folder,
      string fileName);

    Task<bool> DeleteFileAsync(
      string folderName,
      string fileName);

    Task<bool> ExtractFileAsync(
      string folderName,
      string fileName,
      string extractPath,
      CancellationToken token);
  }
}
