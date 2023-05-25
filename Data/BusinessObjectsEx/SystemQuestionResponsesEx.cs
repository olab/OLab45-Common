using Humanizer;
using OLabWebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLabWebAPI.Model
{
  public partial class SystemQuestionResponses
  {
    public object Value { get {

        if (Question == null)
          return null;

        switch (Question.EntryTypeId)
        {
          case (int)SystemQuestionTypes.Type.DragAndDrop:
          case (int)SystemQuestionTypes.Type.DropDown:
            return "";
          case (int)SystemQuestionTypes.Type.Radio:
            return Id;
          case (int)SystemQuestionTypes.Type.MultipleChoice:
            return false;
          default:
            return null;
        }
      } }
  }
}
