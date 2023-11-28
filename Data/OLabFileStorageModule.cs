using OLab.Api.Model;
using OLab.Common.Attributes;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
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

  protected IOLabLogger logger;
  protected IOLabConfiguration cfg;

  protected OLabFileStorageModule(IOLabLogger logger, IOLabConfiguration configuration)
  {
    this.logger = logger;
    this.cfg = configuration;
  }

  public string GetModuleName()
  {
    var attrib = this.GetType().GetCustomAttributes(typeof(OLabModuleAttribute), true).FirstOrDefault() as OLabModuleAttribute;
    if (attrib == null)
      throw new Exception("Missing OLabModule attribute");

    return attrib == null ? "" : attrib.Name;
  }

  public string GetUrlPath(string path, string fileName = null)
  {
    return BuildPath(cfg.GetAppSettings().FileStorageUrl, path, fileName).Replace( "\\", "/" );
  }

  public string GetPhysicalPath(string path, string fileName)
  {
    return BuildPath(cfg.GetAppSettings().FileStorageRoot, path, fileName);
  }

  public string GetPhysicalPath(string path)
  {
    return BuildPath(cfg.GetAppSettings().FileStorageRoot, path);
  }

  public string GetScopedFolderName(string scopeLevel, uint scopeId)
  {
    var scopeFolder = BuildPath(scopeLevel, scopeId);
    return scopeFolder;
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
      sb.Append(pathParts[i].ToString());
      if (i < pathParts.Length - 1)
        sb.Append(GetFolderSeparator());
    }

    // clean up any double separators
    var path = sb.ToString().Replace($"{GetFolderSeparator()}{GetFolderSeparator()}", $"{GetFolderSeparator()}");
    return path;
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
        var scopeFolder = GetScopedFolderName(
          item.ImageableType, 
          item.ImageableId);

        if (FileExists(scopeFolder, item.Path))
        {
          item.OriginUrl = GetUrlPath( 
            scopeFolder,
            item.Path
          );

          logger.LogInformation($"  file '{item.Name}' mapped to url '{item.OriginUrl}'");
        }

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
    string folder,
    string fileName);

  public abstract Task DeleteFolderAsync(string folder);

  public abstract Task<bool> ExtractFileToStorageAsync(
    string folderName,
    string fileName,
    string extractDirectory,
    CancellationToken token);

  public abstract bool FileExists(
    string folder,
    string fileName);

  public abstract IList<string> GetFiles(
    string folderName,
    CancellationToken token);

  public abstract char GetFolderSeparator();

  public abstract Task MoveFileAsync(
    string fileName,
    string sourcePath,
    string destinationPath,
    CancellationToken token = default);

  public abstract Task ReadFileAsync(
    Stream stream,
    string folder,
    string fileName,
    CancellationToken token);

  public abstract Task<string> WriteFileAsync(
    Stream stream,
    string folderName,
    string fileName,
    CancellationToken token);
}
