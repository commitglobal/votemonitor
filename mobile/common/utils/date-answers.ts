/*
  The values of date, time and date-time answers:
  - date-time: the moment the observer picked, as a UTC instant to the minute (no seconds)
  - date: the day as shown to the observer, "YYYY-MM-DDT00:00:00Z" (no time zone shift)
  - time: the time of day as shown to the observer, "HH:mm:ss" (no date, no time zone)
*/

// a date answer is the picked day at midnight UTC ("YYYY-MM-DDT00:00:00Z", or just "YYYY-MM-DD")
const DAY_AT_MIDNIGHT_UTC = /^\d{4}-\d{2}-\d{2}(T00:00(:00(\.0+)?)?(Z|[+-]00:?00)?)?$/;

/**
 * The local day of a date answer. Answers saved before the date picker became a day picker are the
 * full UTC instant that was picked (e.g. 2 May 01:30 in UTC+3 is "2024-05-01T22:30:00Z"): for those
 * the local day of that instant counts, not its first 10 characters (which would be a day earlier).
 * Null when the value is not a date.
 */
export const dayOfIso = (value: string): Date | null => {
  if (!DAY_AT_MIDNIGHT_UTC.test(value)) {
    const instant = new Date(value);
    if (!Number.isNaN(instant.getTime())) {
      return new Date(instant.getFullYear(), instant.getMonth(), instant.getDate());
    }
  }
  if (!/^\d{4}-\d{2}-\d{2}/.test(value)) return null;
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  return new Date(year, month - 1, day);
};

const pad = (n: number) => String(n).padStart(2, "0");

/**
 * The picked moment as a UTC instant, to the minute (pickers only set hours and minutes: leftover
 * seconds are dropped), e.g. "2024-05-01T22:30:00.000Z" for 2 May 01:30 in UTC+3.
 */
export const toDateTimeIso = (date: Date) => {
  const minute = new Date(date);
  minute.setSeconds(0, 0);
  return minute.toISOString();
};

/** The picked local day as "YYYY-MM-DDT00:00:00Z": the API keeps the date part as is, with no time zone shift. */
export const toDayIso = (date: Date) =>
  `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T00:00:00Z`;

/** A date answer back to a local Date on that day (see dayOfIso for answers saved by older versions). */
export const fromDayIso = (value: string) => dayOfIso(value) ?? new Date(NaN);

/** The picked local time as "HH:mm:ss" (a time of day, without a date or time zone). */
export const toTimeOfDay = (date: Date) => `${pad(date.getHours())}:${pad(date.getMinutes())}:00`;

/** A time answer back to a Date today at that time, for the picker. */
export const fromTimeOfDay = (value: string) => {
  const [hours, minutes] = value.split(":").map(Number);
  const date = new Date();
  date.setHours(hours || 0, minutes || 0, 0, 0);
  return date;
};
