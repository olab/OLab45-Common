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
    void AttachUrls(IOLabLogger logger, IList<SystemFiles> items);
    public void MoveFile(IOLabLogger logger, string sourcePath, string destinationPath);
    Task<string> UploadFile(IOLabLogger logger, Stream file,
      string fileName,
      CancellationToken token);
    bool FileExists(IOLabLogger logger, string baseFolder, string physicalFileName);
  }
}
