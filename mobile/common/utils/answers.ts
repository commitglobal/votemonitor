import {
  DateFormat,
  DEFAULT_DATE_FORMAT,
  formatDate,
  formatDayIso,
  formatTime,
  formatTimeOfDay,
} from "./time";
import { ApiFormAnswer } from "../../services/interfaces/answer.type";

/**
 * Returns a display string for a given ApiFormAnswer object.
 *
 * @param {ApiFormAnswer} answer - The ApiFormAnswer object to get the display string for.
 * depending on the answer type, the value has a different key
 *  type date -> date (the day only), shown in the chosen date format
 *  type time -> time ("HH:mm:ss"), shown in the chosen clock format
 *  type dateTime -> dateTime
 *  type text -> text
 *  type number -> value
 *  type rating -> value
 *  type singleSelect -> selection.value
 *  type multipleSelect -> join all selectionValues
 * @return {string} The display string for the given ApiFormAnswer object.
 */

export const getAnswerDisplay = (
  answer: ApiFormAnswer,
  displaySingleSelectOtherText?: boolean,
  formats: { is24Hour: boolean; dateFormat: DateFormat } = {
    is24Hour: true,
    dateFormat: DEFAULT_DATE_FORMAT,
  },
) => {
  if (!answer) return "";

  switch (answer.$answerType) {
    case "dateAnswer":
      // a calendar day: shown as received, without a time or a time zone shift
      return answer.date ? formatDayIso(answer.date, formats.dateFormat) : "";
    case "timeAnswer":
      return answer.time ? formatTimeOfDay(answer.time, formats.is24Hour) : "";
    case "dateTimeAnswer": {
      if (!answer.dateTime) return "";
      const dateTime = new Date(answer.dateTime);
      return `${formatDate(dateTime, formats.dateFormat)} ${formatTime(dateTime, formats.is24Hour)}`;
    }
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
