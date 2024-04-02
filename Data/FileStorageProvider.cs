using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Interface;

namespace OLab.Data;

public class FileStorageProvider : OLabModuleProvider<IFileStorageModule>
{
  public FileStorageProvider(
    IOLabLogger logger,
    IOLabConfiguration configuration) : base(logger, configuration)
  {
    Load("OLab.Files.*.dll");
  }
}
