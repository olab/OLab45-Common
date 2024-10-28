using OLab.Api.Model;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data.Interface;

public interface IFileStorageModule
{
  string GetModuleName();

  char GetFolderSeparator();

  void AttachUrls(
    IList<SystemFiles> items);

  void AttachUrls(
    SystemFiles item);

  public Task MoveFileAsync(
    string sourceFilePath,
    string destinationFolder,
    CancellationToken token = default);

  bool FileExists(
    string filePath);

  Task<bool> ReadFileAsync(
    Stream stream,
    string filePath,
    CancellationToken token);

  Task<string> WriteFileAsync(
    Stream stream,
    string fileName,
    CancellationToken token);

  Task<bool> DeleteFileAsync(
    string filePath);

  Task DeleteFolderAsync(
    string folderName);

  Task<string> ExtractFileToStorageAsync(
    string archiveFileName,
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

  /// <summary>
  /// Builds a path, compatible with the file module
  /// </summary>
  /// <param name="pathParts">Argument list of path parts</param>
  /// <returns>Path string</returns>
  string BuildPath(params object[] pathParts);

  /// <summary>
  /// Calculate target directory for scoped type and id
  /// </summary>
  /// <param name="parentType">Scoped object type (e.g. 'Maps')</param>
  /// <param name="parentId">Scoped object id</param>
  /// <param name="path">Optional file name</param>
  /// <returns>Public directory for scope</returns>
  string GetPublicFileDirectory(string parentType, uint parentId, string fileName = "");

  /// <summary>
  /// Get import file directory 
  /// </summary>
  /// <param name="importFolderName">Import folder name</param>
  /// <returns>Import directory for scope</returns>
  string GetImportMediaFilesDirectory(string importFolderName);
}
