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
            List<string> ids = idList.Split(",").ToList();
            List<uint> iduints = new List<uint>();

            // convert list of strings to list of uints
            foreach (string id in ids)
            {
                if (uint.TryParse(id, out uint iduint))
                    iduints.Add(iduint);
            }

            return GetScoreFromResponses(iduints);
        }

        public int GetScoreFromResponses(IList<uint> ids)
        {
            int score = 0;
            foreach (uint id in ids)
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
