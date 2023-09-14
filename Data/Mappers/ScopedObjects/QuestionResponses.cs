using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.ObjectMapper
{
  public class QuestionResponses : OLabMapper<SystemQuestionResponses, QuestionResponsesDto>
  {
    protected readonly QuestionsFullDto ParentQuestion;

    public QuestionResponses(
      IOLabLogger logger, 
      QuestionsFullDto parentQuestion) : base(logger)
    {
      ParentQuestion = parentQuestion;
    }

    public QuestionResponses(
      IOLabLogger logger, 
      IOLabModuleProvider<IWikiTagModule> tagProvider, 
      QuestionsFullDto parentQuestion) : base(logger, tagProvider)
    {
      ParentQuestion = parentQuestion;
    }

    public QuestionResponses(
      IOLabLogger logger, 
      IOLabModuleProvider<IWikiTagModule> tagProvider, 
      bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override SystemQuestionResponses DtoToPhysical(
      QuestionResponsesDto dto, 
      SystemQuestionResponses phys)
    {
      if (!dto.IsCorrect.HasValue)
        phys.IsCorrect = -1;
      else
        phys.IsCorrect = dto.IsCorrect.Value;

      return phys;
    }

    public override QuestionResponsesDto PhysicalToDto(
      SystemQuestionResponses phys, 
      QuestionResponsesDto dto)
    {
      if (ParentQuestion != null)
      {
        dto.QuestionId = phys.QuestionId.Value;
        dto.IsCorrect = phys.IsCorrect;

        switch (ParentQuestion.EntryTypeId)
        {
          case (int)SystemQuestionTypes.Type.DragAndDrop:
          case (int)SystemQuestionTypes.Type.DropDown:
            dto.Value = "";
            break;
          case (int)SystemQuestionTypes.Type.Radio:
            dto.Value = phys.Id;
            break;
          case (int)SystemQuestionTypes.Type.MultipleChoice:
            dto.Value = false;
            break;
          default:
            dto.Value = null;
            break;
        }
      }

      return dto;
    }

    public override SystemQuestionResponses ElementsToPhys(
      IEnumerable<dynamic> elements, 
      Object source = null)
    {
      var phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      CreateIdTranslation(phys.Id);
      if (uint.TryParse(elements.FirstOrDefault(x => x.Name == "parent_id").Value, out uint id))
        phys.ParentId = id;
      phys.QuestionId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "question_id").Value);
      phys.Response = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "response"));
      phys.Feedback = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "feedback"));

      if (int.TryParse(elements.FirstOrDefault(x => x.Name == "is_correct").Value, out int value))
        phys.IsCorrect = value;
      if (int.TryParse(elements.FirstOrDefault(x => x.Name == "score").Value, out int value1))
        phys.Score = value1;
      phys.Order = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "order").Value);
      phys.From = elements.FirstOrDefault(x => x.Name == "from").Value;
      phys.To = elements.FirstOrDefault(x => x.Name == "to").Value;
      phys.CreatedAt = DateTime.Now;

      // Logger.LogInformation($"loaded SystemQuestionResponses {phys.Id}");

      return phys;
    }

  }
}