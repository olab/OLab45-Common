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
    char GetFolderSeparator();

    void AttachUrls(
      IOLabLogger logger,
      IList<SystemFiles> items);

    public void MoveFile(
      IOLabLogger logger,
      string fileName,
      string sourcePath,
      string destinationPath);

    Task<string> UploadFileAsync(
      IOLabLogger logger,
      Stream stream,
      string fileName,
      CancellationToken token);

    bool FileExists(
      IOLabLogger logger,
      string folder,
      string fileName);

    Task<Stream> ReadFileAsync(
      IOLabLogger logger,
      string folder,
      string fileName);

    Task<bool> DeleteFileAsync(
      IOLabLogger logger,
      string filePath);

    Task<bool> ExtractFileAsync(
      IOLabLogger logger,
      string folderName,
      string fileName,
      string extractPath,
      CancellationToken token);
  }
}
