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

  //void AttachUrls(
  //  IList<SystemFiles> items);

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
    string physicalFilePath,
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

  //string GetPhysicalScopedFilePath(
  //  string scopeLevel,
  //  uint scopeId,
  //  string fileName = null);

  Task<string> WriteScopedFileAsync(
    Stream stream,
    string scopeLevel,
    uint scopeId,
    string fileName,
    CancellationToken token = default);

  string GetWebScopedFilePath(
    string scopeLevel,
    uint scopeId,
    string fileName);

  string GetPhysicalImportFilePath(
    string importName,
    string fileName);

  string GetPhysicalImportMediaFilePath(
    string importName,
    string fileName = "");

  Task MoveImportMediaFileToScopedFolderAsync(
    string importName,
    string fileName, 
    string relativeTargetDirectory);

  /// <summary>
  /// Write import file to storage
  /// </summary>
  /// <param name="stream">File stream</param>
  /// <param name="archiveFileName">Archive file name</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>Physical file name</returns>
  Task<string> WriteImportFileAsync(
    Stream stream, 
    string archiveFileName, 
    CancellationToken token);

  /// <summary>
  /// Read an import file into a stream
  /// </summary>
  /// <param name="importFileDirectory">Import file directoryw</param>
  /// <param name="importFileName">Import file name</param>
  /// <returns>MemoryStream</returns>
  Task<MemoryStream> ReadImportFileAsync(
    string importFileDirectory,
    string importFileName);

  /// <summary>
  /// Delete an import file
  /// </summary>
  /// <param name="importFileName">Import file to delete</param>
  /// <returns>Nothing</returns>
  Task DeleteImportFileAsync(
    string importFileDirectory,
    string importFileName);
}
