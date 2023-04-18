using System.Collections.Generic;
using System.Linq;

namespace OLabWebAPI.Model
{
    public partial class SystemQuestions
    {
        public string GetWikiTag()
        {
            if (string.IsNullOrEmpty(Name))
                return $"[[QU:{Id}]]";
            return $"[[QU:{Name}]]";
        }

        public int GetScoreFromResponses(uint? id)
        {
            if (id.HasValue)
                return GetScoreFromResponses(id.Value);

            return 0;
        }

        public int GetScoreFromResponses(uint id)
        {
            return GetScoreFromResponses(new List<uint> { id });
        }

        public int GetScoreFromResponses(string idList)
        {
            var ids = idList.Split(",").ToList();
            var iduints = new List<uint>();

            // convert list of strings to list of uints
            foreach (var id in ids)
            {
                if (uint.TryParse(id, out var iduint))
                    iduints.Add(iduint);
            }

            return GetScoreFromResponses(iduints);
        }

        public int GetScoreFromResponses(IList<uint> ids)
        {
            var score = 0;
            foreach (var id in ids)
            {
                SystemQuestionResponses response = GetResponse(id);
                if (response != null)
                {
                    if (response.Score.HasValue)
                        score += response.Score.Value;
                }
            }

            return score;
        }

        public SystemQuestionResponses GetResponse(uint id)
        {
            foreach (SystemQuestionResponses item in SystemQuestionResponses)
            {
                if (item.Id == id)
                    return item;
            }

            return null;
        }
    }
}
