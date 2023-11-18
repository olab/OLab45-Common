using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.Interfaces
{
  public interface IImporter
  {
    OLabDBContext GetContext();
    XmlDto GetDto(Importer.DtoTypes type);
    IOLabModuleProvider<IWikiTagModule> GetWikiProvider();
    IFileStorageModule GetFileStorageModule();
    Task Import(Stream stream, string importFileName, CancellationToken token = default);

    AppSettings Settings();
  }
}