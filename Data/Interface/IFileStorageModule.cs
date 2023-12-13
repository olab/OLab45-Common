using OLab.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
      string folderName,
      string fileName,
      CancellationToken token);

    Task<bool> DeleteFileAsync(
      string folder,
      string fileName);

    Task DeleteFolderAsync(
      string folder);

    Task<bool> ExtractFileToStorageAsync(
      string folderName,
      string fileName,
      string extractDirectory,
      CancellationToken token);

    Task<bool> CopyFolderToArchiveAsync(
      ZipArchive archive,
      string folderName,
      string zipEntryFolderName,
      bool appendToStream,
      CancellationToken token);

    IList<string> GetFiles(
      string folderName,
      CancellationToken token);

    string BuildPath(params object[] pathParts);

  }
}
