using OLabWebAPI.Common;
using OLabWebAPI.Dto.Designer;
using OLabWebAPI.Utils;
using System;

namespace OLabWebAPI.ObjectMapper.Designer
{
    public class ScopedObjects : ObjectMapper<Model.ScopedObjects, ScopedObjectsDto>
    {
        protected readonly bool enableWikiTranslation = true;

        public ScopedObjects(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
        {
        }

        public ScopedObjects(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
        {
        }

        public override ScopedObjectsDto PhysicalToDto(Model.ScopedObjects phys, object source = null)
        {
            ScopedObjectsDto dto = GetDto(source);

            System.Collections.Generic.IList<ScopedObjectDto> dtConstantsList = new Constants(logger, new WikiTagProvider(logger)).PhysicalToDto(phys.Constants);
            dto.Constants.AddRange(dtConstantsList);

            System.Collections.Generic.IList<ScopedObjectDto> dtoQuestionsList = new Questions(logger, new WikiTagProvider(logger)).PhysicalToDto(phys.Questions);
            dto.Questions.AddRange(dtoQuestionsList);

            System.Collections.Generic.IList<ScopedObjectDto> dtCountersList = new Counters(logger, new WikiTagProvider(logger)).PhysicalToDto(phys.Counters);
            dto.Counters.AddRange(dtCountersList);

            System.Collections.Generic.IList<ScopedObjectDto> dtFilesList = new Files(logger, new WikiTagProvider(logger)).PhysicalToDto(phys.Files);
            dto.Files.AddRange(dtFilesList);

            return dto;
        }

        public override Model.ScopedObjects DtoToPhysical(ScopedObjectsDto dto, object source = null)
        {
            throw new NotImplementedException();
        }

    }
}