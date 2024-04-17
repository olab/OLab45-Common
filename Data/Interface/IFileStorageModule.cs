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

  public Task MoveFileAsync(
    string relativeSourceFilePath,
    string relativeDestinationFolder,
    CancellationToken token = default);

  bool FileExists(
    string relativeFilePath);

  Task ReadFileAsync(
    Stream stream,
    string relativeFilePath,
    CancellationToken token);

  Task<string> WriteFileAsync(
    Stream stream,
    string relativeFilePath,
    CancellationToken token);

  Task<bool> DeleteFileAsync(
    string relativeFilePath);

  Task DeleteFolderAsync(
    string relativeFolderName);

  Task<bool> ExtractFileToStorageAsync(
    string relativeArchiveFilePath,
    string relativeExtractDirectory,
    CancellationToken token);

  Task<bool> CopyFolderToArchiveAsync(
    ZipArchive archive,
    string relativeFileDirectory,
    string zipEntryFolderName,
    bool appendToStream,
    CancellationToken token);

  IList<string> GetFiles(
    string relativeFileDirectory,
    CancellationToken token);

  /// <summary>
  /// Builds a path, compatible with the file module
  /// </summary>
  /// <param name="pathParts">Argument list of path parts</param>
  /// <returns>Path string</returns>
  string BuildPath(params object[] pathParts);

  string GetPhysicalScopedFilePath(
    string scopeLevel,
    uint scopeId,
    string fileName);

  string GetWebScopedFilePath(
    string scopeLevel,
    uint scopeId,
    string fileName);

  string GetPhysicalImportFilePath(
    string importName,
    string fileName);

  string GetPhysicalImportMediaFilePath(
    string importName,
    string fileName);
}
