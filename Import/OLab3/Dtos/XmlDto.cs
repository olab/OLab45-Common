using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OLab.Import.OLab3.Importer;

namespace OLab.Import.OLab3.Dtos;

public abstract class XmlDto
{
  protected readonly IOLabLogger Logger;
  public abstract Task<bool> LoadAsync(string extractPath, bool displayProgressMessage = true);
  public abstract bool PostProcess(IDictionary<DtoTypes, XmlDto> dtos);
  public abstract bool SaveToDatabase(string importFolderName);
  public abstract object GetDbPhys();

  public DtoTypes DtoType { get; private set; }

  // used to hold on to id translation between origin system and new one
  protected IDictionary<uint, uint?> _idTranslation = new Dictionary<uint, uint?>();
  protected IList<IEnumerable<dynamic>> xmlImportElementSets = new List<IEnumerable<dynamic>>();

  public XmlDto(IOLabLogger logger, DtoTypes dtoType)
  {
    DtoType = dtoType;
    Logger = logger;
  }

  protected abstract bool CreateIdTranslation(uint originalId, uint? newId = null);
  public abstract uint? GetIdTranslation(string referencedFile, uint originalId);
  public abstract int? GetIdTranslation(string referencedFile, int? originalId);
}