using OLab.Api.Model;
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

    Task CopyFileToStreamAsync(
      Stream stream,
      string folder,
      string fileName,
      CancellationToken token);

    Task<string> CopyStreamToFileAsync(
      Stream stream,
      string filePath,
      CancellationToken token);

    Task<bool> DeleteFileAsync(
      string folder,
      string fileName);

    Task<bool> ExtractFileToStorageAsync(
      string folderName,
      string fileName,
      string extractDirectory,
      CancellationToken token);

    Task<bool> CopyFoldertoArchiveAsync(
      ZipArchive archive,
      string folderName,
      bool appendToStream,
      CancellationToken token);
  }
}
