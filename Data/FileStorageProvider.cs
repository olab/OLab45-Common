using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Data.Interface;

namespace OLab.Data
{
  public class FileStorageProvider : OLabModuleProvider<IFileStorageModule>
  {
    public FileStorageProvider(OLabLogger logger) : base(logger)
    {
      Load("OLab.Files.*.dll");
    }
  }
}
