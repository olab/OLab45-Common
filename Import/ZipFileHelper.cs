using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

    if ( ZipArchive.IsZipFile( stream ) )
    {
      var zipReaderOptions = new ReaderOptions
      {
        ArchiveEncoding = new ArchiveEncoding
        {
          Default = Encoding.UTF8,
          Forced = Encoding.UTF8
        },
        LookForHeader = true
      };

      stream.Position = 0;

      using var reader = ReaderFactory.OpenReader( stream, zipReaderOptions );

      while ( reader.MoveToNextEntry() )
      {
        var entry = reader.Entry;

        if ( !entry.IsDirectory )
          files.Add( entry.Key );
      }

      stream.Position = 0;
    }

    return files;
  }

  /// <summary>
  /// Get file entries from zip file stream
  /// </summary>
  /// <param name="stream">Zip file stream</param>
  /// <returns>List of zip archive entries</returns>
  public static IList<IEntry> GetFileEntries(Stream stream)
  {
    var entries = new List<IEntry>();

    if ( ZipArchive.IsZipFile( stream ) )
    {
      var zipReaderOptions = new ReaderOptions
      {
        ArchiveEncoding = new ArchiveEncoding
        {
          Default = Encoding.UTF8,
          Forced = Encoding.UTF8
        },
        LookForHeader = true
      };

      stream.Position = 0;

      using var reader = ReaderFactory.OpenReader( stream, zipReaderOptions );

      while ( reader.MoveToNextEntry() )
      {
        var entry = reader.Entry;

        if ( !entry.IsDirectory )
          entries.Add( entry );
      }

      stream.Position = 0;
    }

    return entries;
  }

}
