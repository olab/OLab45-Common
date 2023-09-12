using OLab.Api.Common;
using OLab.Api.Importer;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;

namespace OLab.Import.Interfaces
{
  public interface IImporter
  {
    OLabDBContext GetContext();
    XmlDto GetDto(Importer.DtoTypes type);
    IOLabLogger GetLogger();
    WikiTagProvider GetWikiProvider();
    bool LoadAll(string archiveFileName);
    void LogDebug(string message);
    void LogError(Exception ex, string message);
    void LogError(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    bool SaveAll();
    AppSettings Settings();
  }
}