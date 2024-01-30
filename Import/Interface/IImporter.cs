using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Interface;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.Interface;

public interface IImporter
{
  OLabDBContext GetDbContext();
  IOLabModuleProvider<IWikiTagModule> GetWikiProvider();
  IFileStorageModule GetFileStorageModule();
  Task<uint> Import(Stream stream, string fileName, CancellationToken token = default);
  Task ExportAsync(Stream stream, uint mapId, CancellationToken token = default);
  Task<MapsFullRelationsDto> ExportAsync(uint mapId, CancellationToken token = default);

  AppSettings Settings();
}