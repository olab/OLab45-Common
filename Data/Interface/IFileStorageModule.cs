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

    bool FileExists(
      string folder,
      string fileName);

    Task ReadFileAsync(
      Stream stream,
      string folder,
      string fileName,
      CancellationToken token);

    Task<string> WriteFileAsync(
      Stream stream,
      string filePath,
      CancellationToken token);

    Task<bool> DeleteFileAsync(
      string folder,
      string fileName);

    Task<bool> ExtractFileAsync(
      string folderName,
      string fileName,
      string extractDirectory,
      CancellationToken token);
  }
}
