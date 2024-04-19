using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data;
using OLab.Data.Interface;
using System.Collections.Generic;

namespace OLab.Import.OLab4;

public partial class Importer : OLabImporter
{
  public const string MapFileName = "map.json";

  private readonly IDictionary<uint, uint?> _nodeIdTranslation = new Dictionary<uint, uint?>();

  public Importer(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IFileStorageModule fileStorageModule) : base(
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageModule)
  {
  }

}