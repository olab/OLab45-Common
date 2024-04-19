using OLab.Api.Common;
using OLab.Api.Data.Interface;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using OLab.Data;
using OLab.Data.Interface;
using OLab.Import.Interface;
using OLab.Import.OLab3.Dtos;
using OLab.Import.OLab3.Model;
using OLab.Import.OLab4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Import.OLab3;

public partial class Importer : OLabImporter
{
  public enum DtoTypes
  {
    XmlMapDto,
    XmlMapNodeDto,
    XmlMapNodeLinkDto,
    XmlMapCounterDto,
    XmlMapCounterRuleDto,
    XmlMapElementDto,
    XmlMapNodeCounterDto,
    XmlMapQuestionDto,
    XmlMapQuestionResponseDto,
    XmlMapNodeSectionDto,
    XmlMapNodeSectionNodeDto,
    XmlMapVpdDto,
    XmlMapVpdElementDto,
    XmlMediaElementsDto,
    XmlMetadataDto,
    XmlManifestDto,
    XmlMapAvatarDto
  };

  private readonly IDictionary<DtoTypes, XmlDto> _dtos = new Dictionary<DtoTypes, XmlDto>();
  public XmlDto GetDto(DtoTypes type) { return _dtos[type]; }

  public Importer(
    IOLabLogger logger,
    IOLabConfiguration configuration,
    OLabDBContext context,
    IOLabModuleProvider<IWikiTagModule> wikiTagProvider,
    IFileStorageModule fileStorageModule) : base (
      logger,
      configuration,
      context,
      wikiTagProvider,
      fileStorageModule)
  {
    XmlDto dto = new XmlMapDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapElementDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMediaElementsDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapAvatarDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapVpdDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapVpdElementDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapCounterDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapQuestionDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapQuestionResponseDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeCounterDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeLinkDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    //dto = new XmlMapCounterRuleDto(Logger, this);
    //_dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeSectionDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    dto = new XmlMapNodeSectionNodeDto(Logger, this);
    _dtos.Add(dto.DtoType, dto);

    //dto = new XmlMetadataDto(Logger, this);
    //_dtos.Add(dto.DtoType, dto);

    //dto = new XmlManifestDto(Logger, this);
    //_dtos.Add(dto.DtoType, dto);
  }

  public override Task ExportAsync(Stream stream, uint mapId, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }

  public override Task<MapsFullRelationsDto> ExportAsync(uint mapId, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }
}