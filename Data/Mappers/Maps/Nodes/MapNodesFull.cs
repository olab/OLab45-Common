using AutoMapper;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using OLab.Data.ReaderWriters;

namespace OLab.Api.ObjectMapper;

public class MapNodesFullMapper : OLabMapper<MapNodes, MapNodesFullDto>
{
  protected readonly bool enableWikiTranslation = false;
  private readonly QuestionReaderWriter _reader;
  private readonly MapNodeGroupRolesMapper _groupRoleMapper;

  public MapNodesFullMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
    this.enableWikiTranslation = enableWikiTranslation;

    _reader = new QuestionReaderWriter(
      GetLogger(),
      GetDbContext(),
      tagProvider );

    _groupRoleMapper = new MapNodeGroupRolesMapper( GetLogger(), GetDbContext(), tagProvider );
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration( cfg =>
    {
      cfg.CreateMap<MapNodes, MapNodesFullDto>().ReverseMap();
      cfg.CreateMap<MapNodeLinks, MapNodeLinksFullDto>().ReverseMap();
      cfg.CreateMap<MapNodeGrouproles, MapNodeGroupRolesDto>().ReverseMap();
    } );
  }

  public override MapNodes DtoToPhysical(MapNodesFullDto dto, MapNodes phys)
  {
    // patch up node size, just in case it's not set properly
    if ( phys.Height == 0 )
      phys.Height = 440;

    if ( phys.Width == 0 )
      phys.Width = 300;

    phys.Rgb = dto.Color;
    phys.MapNodeGrouproles.Clear();

    foreach ( var groupRoleDto in dto.MapNodeGrouproles )
      phys.MapNodeGrouproles.Add( _groupRoleMapper.DtoToPhysical( groupRoleDto ) );

    return phys;
  }

  public override MapNodesFullDto PhysicalToDto(MapNodes phys, MapNodesFullDto dto)
  {
    dto.Height = phys.Height.HasValue ? phys.Height : MapNodesMapper.DefaultHeight;
    dto.Text = phys.Text;

    if ( enableWikiTranslation )
    {
      dto.Text = _reader.DisambiguateWikiQuestions( phys.Id, phys.MapId, dto.Text );
      dto.Text = GetWikiProvider().Translate( dto.Text );
    }
    else

      dto.Width = phys.Width.HasValue ? phys.Width : MapNodesMapper.DefaultWidth;
    dto.Color = phys.Rgb;

    if ( string.IsNullOrEmpty( dto.Color ) )
      dto.Color = "#F78749";

    dto.MapNodeGrouproles.Clear();
    foreach ( var groupRolePhys in phys.MapNodeGrouproles )
      dto.MapNodeGrouproles.Add( _groupRoleMapper.PhysicalToDto( groupRolePhys ) );

    return dto;
  }
}
