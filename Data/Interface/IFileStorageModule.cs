using OLab.Api.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Data.Interface
{
  public interface IFileStorageModule
  {
    void AttachUrls(IList<SystemFiles> items);
    public void MoveFile(string sourcePath, string destinationPath);
    Task<string> UploadFile(Stream file,
      string fileName,
      CancellationToken token);
    bool FileExists(string physicalPath);
  }
}
