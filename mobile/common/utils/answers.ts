import { format } from "date-fns";
import { ApiFormAnswer } from "../../services/interfaces/answer.type";

/**
 * Returns a display string for a given ApiFormAnswer object.
 *
 * @param {ApiFormAnswer} answer - The ApiFormAnswer object to get the display string for.
 * depending on the answer type, the value has a different key
 *  type date -> date
 *  type text -> text
 *  type number -> value
 *  type rating -> value
 *  type singleSelect -> selection.value
 *  type multipleSelect -> join all selectionValues
 * @return {string} The display string for the given ApiFormAnswer object.
 */

export const getAnswerDisplay = (answer: ApiFormAnswer, displaySingleSelectOtherText?: boolean) => {
  if (!answer) return "";

  switch (answer.$answerType) {
    case "dateAnswer":
      const date = new Date(answer.date); // eslint-disable-line no-case-declarations
      return format(date, "yyyy-MM-dd HH:mm");
    case "textAnswer":
      return answer.text;
    case "singleSelectAnswer":
      return displaySingleSelectOtherText && answer.selection.text
        ? `${answer.selection.value} (${answer.selection.text.trim()})`
        : answer.selection.value;
    case "multiSelectAnswer":
      return answer.selectionValues?.join("; ") || "";
    case "numberAnswer":
    case "ratingAnswer":
      return answer.value?.toString() || "";
    default:
      return "";
  }
};
