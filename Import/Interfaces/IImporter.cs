using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System;
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
    Task<bool> ProcessImportFileAsync(string archiveFileName, CancellationToken token);
    void LogDebug(string message);
    void LogError(Exception ex, string message);
    void LogError(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    bool WriteImportToDatabase();
    AppSettings Settings();
  }
}