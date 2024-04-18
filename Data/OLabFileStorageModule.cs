using Humanizer;
using OLab.Api.Model;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using OLabWeOLabWebAPI.ObjectMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Common.Utils;
public abstract class OLabFileStorageModule : IFileStorageModule
{
  public const string FilesRoot = "files";
  public const string ImportRoot = "import";
  public const string MediaDirectory = "media";

  protected IOLabLogger logger;
  protected IOLabConfiguration cfg;

  protected OLabFileStorageModule(IOLabLogger logger, IOLabConfiguration configuration)
  {
    this.logger = logger;
    this.cfg = configuration;
  }

  public string GetPhysicalPath(string path, string fileName = "")
  {
    return BuildPath(cfg.GetAppSettings().FileStorageRoot, path, fileName);
  }

  public string GetPhysicalScopedFilePath(
  string scopeLevel,
  uint scopeId,
  string fileName)
  {
    return BuildPath(
      cfg.GetAppSettings().FileStorageRoot,
      FilesRoot,
      scopeLevel,
      scopeId,
      fileName);
  }

  public string GetWebScopedFilePath(string scopeLevel, uint scopeId, string fileName)
  {
    var physFilePath = GetPhysicalScopedFilePath(scopeLevel, scopeId, fileName);
    if (!FileExists(physFilePath))
      return string.Empty;

    var webFilePath = BuildPath(
      cfg.GetAppSettings().FileStorageUrl,
      FilesRoot,
      scopeLevel,
      scopeId,
      fileName);

    webFilePath = webFilePath.Replace('\\', '/');
    return webFilePath;
  }

  public string GetPhysicalImportFilePath(
    string importName,
    string fileName = "")
  {
    return BuildPath(
      cfg.GetAppSettings().FileStorageRoot,
      ImportRoot,
      importName,
      fileName);
  }

  public string GetPhysicalImportMediaFilePath(string importName, string fileName)
  {
    return BuildPath(
      cfg.GetAppSettings().FileStorageRoot,
      ImportRoot,
      importName,
      MediaDirectory,
      fileName);
  }

  public string GetModuleName()
  {
    var attrib =
      this.GetType().GetCustomAttributes(typeof(OLabModuleAttribute), true).FirstOrDefault() as OLabModuleAttribute;
    if (attrib == null)
      throw new Exception("Missing OLabModule attribute");

    return attrib == null ? "" : attrib.Name;
  }

  /// <summary>
  /// Builds a path, compatible with the file module
  /// </summary>
  /// <param name="pathParts">Argument list of path parts</param>
  /// <returns>Path string</returns>
  public string BuildPath(params object[] pathParts)
  {
    var sb = new StringBuilder();
    for (int i = 0; i < pathParts.Length; i++)
    {
      // remove any extra trailing slashes
      var part = pathParts[i].ToString();
      if (string.IsNullOrEmpty(part))
        continue;
      part = part.TrimEnd(GetFolderSeparator());

      sb.Append(part);
      if (i < pathParts.Length - 1)
        sb.Append(GetFolderSeparator());
    }

    return sb.ToString().TrimEnd(GetFolderSeparator());
  }

  /// <summary>
  /// Attach browseable URLS for system files
  /// </summary>
  /// <param name="items">List of system files objects to enhance</param>
  public void AttachUrls(IList<SystemFiles> items)
  {
    if (items.Count == 0)
      return;

    logger.LogInformation($"Attaching URLs for {items.Count} file records");

    foreach (var item in items)
    {
      try
      {
        item.OriginUrl = GetWebScopedFilePath(
          item.ImageableType,
          item.ImageableId,
          item.Name);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, $"AttachUrls error on '{item.Path}' id = {item.Id}");
      }

    }
  }

  public abstract Task<bool> CopyFolderToArchiveAsync(
    ZipArchive archive,
    string folderName,
    string zipEntryFolderName,
    bool appendToStream,
    CancellationToken token);

  public abstract Task<bool> DeleteFileAsync(
    string filePath);

  public abstract Task DeleteFolderAsync(
    string folderName);

  public abstract Task<bool> ExtractFileToStorageAsync(
    string archiveFileName,
    string extractDirectory,
    CancellationToken token);

  public abstract bool FileExists(
    string filePath);

  public abstract IList<string> GetFiles(
    string folderName,
    CancellationToken token);

  public abstract char GetFolderSeparator();

  public abstract Task MoveFileAsync(
    string physSourceFilePath,
    string physDestinationFolder,
    CancellationToken token = default);

  public abstract Task ReadFileAsync(
    Stream stream,
    string fileName,
    CancellationToken token);

  /// <summary>
  /// Uploads a file represented by a stream to a directory
  /// </summary>
  /// <param name="file">File contents stream</param>
  /// <param name="physicalFilePath">Physical file path to write to</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>Physical file path</returns>
  public abstract Task<string> WriteFileAsync(
    Stream stream,
    string physicalFilePath,
    CancellationToken token);


  public async Task<string> WriteScopedFileAsync(
    Stream stream,
    string scopeLevel,
    uint scopeId,
    string fileName,
    CancellationToken token = default)
  {
    var physFilePath = GetPhysicalScopedFilePath(
      scopeLevel,
      scopeId,
      fileName);

    return await WriteFileAsync(
      stream,
      physFilePath,
      token);
  }

  /// <summary>
  /// Move import media file to scoped file directory
  /// </summary>
  /// <param name="fileName">Improt media file name to move</param>
  /// <param name="relativeTargetFolder">Target relative scoped directory</param>
  /// <returns></returns>
  public async Task MoveImportMediaFileToScopedFolderAsync(
    string importName,
    string fileName,
    string relativeTargetFolder)
  {
    var physFilePath = GetPhysicalImportMediaFilePath(
      importName,
      fileName);

    var physTargetFolder = GetPhysicalPath(relativeTargetFolder);

    await MoveFileAsync(
      physFilePath,
      physTargetFolder);
  }

  /// <summary>
  /// Write import file to storage
  /// </summary>
  /// <param name="stream">File stream</param>
  /// <param name="archiveFileName">Archive file name</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>Relative file name</returns>
  public async Task<string> WriteImportFileAsync(
  Stream stream,
    string archiveFileName,
    CancellationToken token)
  {
    var physArchiveFilePath = GetPhysicalImportFilePath(archiveFileName);

    await WriteFileAsync(
      stream,
      physArchiveFilePath,
      token);

    // build extract direct based on archive file name without extension
    var physExtractFolder =
      BuildPath(
        cfg.GetAppSettings().FileStorageRoot,
        ImportRoot,
        Path.GetFileNameWithoutExtension(archiveFileName));
    logger.LogInformation($"Folder extract directory: {physExtractFolder}");

    // extract archive file to extract directory
    await ExtractFileToStorageAsync(
      physArchiveFilePath,
      physExtractFolder,
      token);

    return BuildPath(ImportRoot, archiveFileName);

  }

  /// <summary>
  /// REad an import file into a stream
  /// </summary>
  /// <param name="importFolder">Import folder name</param>
  /// <param name="importFileName">Import file name</param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<MemoryStream> ReadImportFileAsync(
    string importFolder,
    string importFileName)
  {
    var physFilePath = GetPhysicalImportFilePath(
      importFolder,
      importFileName);

    if (FileExists(physFilePath))
    {
      var moduleFileStream = new MemoryStream();
      await ReadFileAsync(
        moduleFileStream,
        physFilePath,
        new CancellationToken());

      moduleFileStream.Position = 0;
      return moduleFileStream;
    }

    throw new Exception($"'{physFilePath}' file not found");
  }

  /// <summary>
  /// Delete an import file
  /// </summary>
  /// <param name="importFileName">Import file to delete</param>
  /// <returns>Nothing</returns>
  public async Task DeleteImportFileAsync(
    string importFileDirectory,
    string importFileName)
  {
    var physFilePath = GetPhysicalImportFilePath(
      importFileDirectory, 
      importFileName);

    await DeleteFileAsync(physFilePath);
  }
}
