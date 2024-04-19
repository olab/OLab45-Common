using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives.Zip;
using System.Threading;

namespace OLab.Import;

public static class ZipFileHelper
{
  /// <summary>
  /// Get files from a zip file stream
  /// </summary>
  /// <param name="stream">Zip file stream</param>
  /// <returns>List of zip file entries</returns>
  public static IList<string> GetFiles(Stream stream)
  {
    var files = new List<string>();

    if (ZipArchive.IsZipFile(stream))
    {
      var zipReaderOptions = new ReaderOptions()
      {
        ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
        LookForHeader = true
      };

      stream.Position = 0;

      using var reader = ZipArchive.Open(stream, zipReaderOptions);

      foreach (var archiveEntry in reader.Entries.Where(entry => !entry.IsDirectory))
        files.Add( archiveEntry.Key );

      stream.Position = 0;
    }

    return files;
  }

  /// <summary>
  /// Get file entries from zip file stream
  /// </summary>
  /// <param name="stream">Zip file stream</param>
  /// <returns>List of zip archive entries</returns>
  public static IList<ZipArchiveEntry> GetFileEntries(Stream stream)
  {
    var entries = new List<ZipArchiveEntry>();

    if (ZipArchive.IsZipFile(stream))
    {
      var zipReaderOptions = new ReaderOptions()
      {
        ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
        LookForHeader = true
      };

      stream.Position = 0;

      using var reader = ZipArchive.Open(stream, zipReaderOptions);
      entries = reader.Entries.ToList();

      stream.Position = 0;

    }

    return entries;
  }
}
