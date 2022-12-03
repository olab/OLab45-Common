using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;

namespace OLabWebAPI.ObjectMapper
{
    public class FilesFull : OLabMapper<SystemFiles, FilesFullDto>
    {

        public FilesFull(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
        {
        }

        public FilesFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
        {
        }

    }
}