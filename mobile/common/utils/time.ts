import { format } from "date-fns";
import { dayOfIso } from "./date-answers";

// the date formats the user can pick in settings, as date-fns patterns
export const DATE_FORMATS = ["yyyy-MM-dd", "dd.MM.yyyy", "dd/MM/yyyy", "MM/dd/yyyy"] as const;
export type DateFormat = (typeof DATE_FORMATS)[number];
export const DEFAULT_DATE_FORMAT: DateFormat = "yyyy-MM-dd";

/** A date-fns pattern as users know it, e.g. "yyyy-MM-dd" -> "YYYY-MM-DD". */
export const getDateFormatLabel = (dateFormat: DateFormat) => dateFormat.toUpperCase();

/** A time of the day, e.g. "13:05" or "1:05 PM". */
export const formatTime = (value: Date, is24Hour: boolean) =>
  format(value, is24Hour ? "HH:mm" : "h:mm a");

/** A calendar day in the chosen date format, e.g. "2026-11-03" or "03.11.2026". */
export const formatDate = (value: Date, dateFormat: DateFormat) => format(value, dateFormat);

/** A date answer (see dayOfIso), shown in the chosen date format without a time zone shift. */
export const formatDayIso = (day: string, dateFormat: DateFormat) => {
  const date = dayOfIso(day);
  return date ? formatDate(date, dateFormat) : day.slice(0, 10);
};

/** A wall-clock time stored as "HH:mm" or "HH:mm:ss", shown in the chosen clock format. */
export const formatTimeOfDay = (time: string, is24Hour: boolean) => {
  const [hours, minutes] = time.split(":").map(Number);
  if (Number.isNaN(hours) || Number.isNaN(minutes)) return time.slice(0, 5);
  return formatTime(new Date(2000, 0, 1, hours, minutes), is24Hour);
};

/**
 * The iOS picker has no 24-hour prop: its clock follows the region of its locale.
 * Keep the language (for month names) and pick a region with the wanted clock.
 */
export const getPickerLocale = (language: string, is24Hour: boolean) =>
  `${language.split("-")[0]}_${is24Hour ? "GB" : "US"}`;
