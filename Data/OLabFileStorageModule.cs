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
  public const string MediaDirectory = "media";

  protected IOLabLogger logger;
  protected IOLabConfiguration cfg;

  public abstract string GetUrlPath(string path, string fileName);

  protected OLabFileStorageModule(IOLabLogger logger, IOLabConfiguration configuration)
  {
    this.logger = logger;
    this.cfg = configuration;
  }

  public string GetModuleName()
  {
    var attrib =
      this.GetType().GetCustomAttributes( typeof( OLabModuleAttribute ), true ).FirstOrDefault() as OLabModuleAttribute;
    if ( attrib == null )
      throw new Exception( "Missing OLabModule attribute" );

    return attrib == null ? "" : attrib.Name;
  }

  public string GetPhysicalPath(string path, string fileName)
  {
    return BuildPath( cfg.GetAppSettings().FileStorageRoot, path, fileName );
  }

  public string GetPhysicalPath(string path)
  {
    return BuildPath( cfg.GetAppSettings().FileStorageRoot, path );
  }

  public string GetScopedFolderName(string scopeLevel, uint scopeId)
  {
    var scopeFolder = BuildPath( scopeLevel, scopeId );
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
    for ( var i = 0; i < pathParts.Length; i++ )
    {
      // remove any extra trailing slashes
      var part = pathParts[ i ].ToString();
      if ( string.IsNullOrEmpty( part ) )
        continue;
      part = part.TrimEnd( GetFolderSeparator() );

      sb.Append( part );
      if ( i < pathParts.Length - 1 )
        sb.Append( GetFolderSeparator() );
    }

    return sb.ToString();
  }

  public void AttachUrls(SystemFiles item)
  {
    var scopeFolder = GetScopedFolderName(
      item.ImageableType,
      item.ImageableId );

    var filePath = BuildPath( FilesRoot, scopeFolder, item.Path );

    if ( FileExists( filePath ) )
    {
      item.OriginUrl = GetUrlPath(
        scopeFolder,
        item.Path
      );

      logger.LogInformation( $"  file '{item.Name}' mapped to url '{item.OriginUrl}'" );
    }

  }

  /// <summary>
  /// Attach browseable URLS for system files
  /// </summary>
  /// <param name="items">List of system files objects to enhance</param>
  public void AttachUrls(IList<SystemFiles> items)
  {
    if ( items.Count == 0 )
      return;

    logger.LogInformation( $"Attaching URLs for {items.Count} file records" );

    foreach ( var item in items )
    {
      try
      {
        AttachUrls( item );
      }
      catch ( Exception ex )
      {
        logger.LogError( ex, $"AttachUrls error on '{item.Path}' id = {item.Id}" );
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

  public abstract Task<string> ExtractFileToStorageAsync(
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
    string sourceFilePath,
    string destinationFolder,
    CancellationToken token = default);

  public abstract Task<bool> ReadFileAsync(
    Stream stream,
    string fileName,
    CancellationToken token);

  public abstract Task<string> WriteFileAsync(
    Stream stream,
    string fileName,
    CancellationToken token);

  /// <summary>
  /// Calculate physical target directory for scoped type and id
  /// </summary>
  /// <param name="parentType">Scoped object type (e.g. 'Maps')</param>
  /// <param name="parentId">Scoped object id</param>
  /// <param name="fileName">Optional file name</param>
  /// <returns>Public directory for scope</returns>
  public string GetPublicFileDirectory(string parentType, uint parentId, string fileName = "")
  {
    var physicalDirectory = BuildPath(
      cfg.GetAppSettings().FileStorageRoot,
      FilesRoot,
      parentType,
      parentId.ToString() );

    if ( !string.IsNullOrEmpty( fileName ) )
      physicalDirectory = BuildPath( physicalDirectory, fileName );

    return physicalDirectory;
  }

  public string GetImportMediaFilesDirectory(string importFolderName)
  {
    return BuildPath( importFolderName, MediaDirectory );
  }
}
