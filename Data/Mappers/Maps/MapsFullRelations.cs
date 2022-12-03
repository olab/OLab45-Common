using AutoMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System.Collections.Generic;
using System.Linq;

namespace OLabWebAPI.ObjectMapper
{
    public class MapsFullRelationsMapper : OLabMapper<Model.Maps, MapsFullRelationsDto>
    {
        public MapsFullRelationsMapper(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
        {
        }

        public MapsFullRelationsMapper(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
        {
        }

        /// <summary>
        /// Default (overridable) AutoMapper configuration
        /// </summary>
        /// <returns>MapperConfiguration</returns>
        protected override MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.Maps, MapsFullRelationsDto>().ReverseMap();
                cfg.CreateMap<Model.MapNodes, MapNodesFullDto>().ReverseMap();
                cfg.CreateMap<Model.MapNodeLinks, MapNodeLinksFullDto>().ReverseMap();
            });
        }

        /// <summary>
        /// Convert a physical object to new dto. 
        /// </summary>
        /// <remarks>
        /// Allows for derived class specific overrides that 
        /// don't fit well with default implementation
        /// </remarks>
        /// <param name="phys">Physical object</param>
        /// <returns>Dto object</returns>
        public override MapsFullRelationsDto PhysicalToDto(Model.Maps map)
        {
            MapsFullRelationsDto dto = new MapsFullRelationsDto
            {
                Map = new MapsFullMapper(logger).PhysicalToDto(map),
                MapNodes = new MapNodesFullMapper(logger).PhysicalToDto(map.MapNodes.ToList())
            };

            List<MapNodeLinks> links = new List<MapNodeLinks>();
            foreach (MapNodes node in map.MapNodes)
                links.AddRange(node.MapNodeLinksNodeId1Navigation);

            dto.MapNodeLinks = new MapNodeLinksMapper(logger).PhysicalToDto(links);

            return dto;
        }

    }
}
