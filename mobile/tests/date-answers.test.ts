/*
  What date, time and date-time questions send and read back, in the observer's own time zone:
  - date-time: the picked moment as a UTC instant, to the minute
  - date: the day shown, with no time zone shift
  - time: the time shown, with no time zone shift
  Node applies a change of process.env.TZ right away, so each case runs in several time zones.
*/
import { describe, test } from "node:test";
import assert from "node:assert/strict";
import {
  dayOfIso,
  fromDayIso,
  fromTimeOfDay,
  toDateTimeIso,
  toDayIso,
  toTimeOfDay,
} from "../common/utils/date-answers.ts";

const TIME_ZONES = [
  "UTC",
  "Europe/Bucharest", // UTC+2/+3
  "America/New_York", // UTC-5/-4
  "Asia/Kolkata", // UTC+5:30
  "Asia/Tokyo", // UTC+9
  "Pacific/Kiritimati", // UTC+14
  "Pacific/Pago_Pago", // UTC-11
];

const inEachTimeZone = (name: string, fn: () => void) => {
  for (const timeZone of TIME_ZONES) {
    test(`${name} [${timeZone}]`, () => {
      const previous = process.env.TZ;
      process.env.TZ = timeZone;
      try {
        fn();
      } finally {
        process.env.TZ = previous;
      }
    });
  }
};

const ymd = (d: Date | null) =>
  d &&
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;

// the pickers return a local Date: the observer picks 2 May 2024 at 00:30, 13:05 or 23:45 on screen
// (with seconds left over, as the Android picker does)
const picked = () =>
  [
    [0, 30],
    [13, 5],
    [23, 45],
  ].map(([h, m]) => new Date(2024, 4, 2, h, m, 17, 250));

describe("date questions send the day shown", () => {
  inEachTimeZone("sent value", () => {
    for (const date of picked()) {
      const sent = toDayIso(date);
      assert.equal(sent, "2024-05-02T00:00:00Z");
      // the API (a DateTime) reads it as midnight UTC: still 2 May
      assert.equal(new Date(sent).toISOString().slice(0, 10), "2024-05-02");
      // read back: 2 May on screen
      assert.equal(ymd(fromDayIso(sent)), "2024-05-02");
    }
  });
});

describe("time questions send the time shown", () => {
  inEachTimeZone("sent value, without seconds", () => {
    assert.deepEqual(picked().map(toTimeOfDay), ["00:30:00", "13:05:00", "23:45:00"]);
    for (const date of picked()) {
      const back = fromTimeOfDay(toTimeOfDay(date));
      assert.equal(
        `${back.getHours()}:${back.getMinutes()}`,
        `${date.getHours()}:${date.getMinutes()}`,
      );
    }
  });
});

describe("date-time questions send the picked moment in UTC, to the minute", () => {
  inEachTimeZone("sent value", () => {
    for (const date of picked()) {
      const sent = toDateTimeIso(date);
      assert.match(sent, /:00\.000Z$/);
      const minute = new Date(date);
      minute.setSeconds(0, 0);
      assert.equal(new Date(sent).getTime(), minute.getTime());
      // shown back in the observer's time zone as what they picked
      assert.equal(new Date(sent).getHours(), date.getHours());
      assert.equal(date.getSeconds(), 17); // the picked Date itself is not changed
    }
  });

  test("2 May 01:30 in Bucharest is sent as 1 May 22:30 UTC", () => {
    const previous = process.env.TZ;
    process.env.TZ = "Europe/Bucharest";
    try {
      assert.equal(toDateTimeIso(new Date(2024, 4, 2, 1, 30, 42, 123)), "2024-05-01T22:30:00.000Z");
    } finally {
      process.env.TZ = previous;
    }
  });
});

describe("date answers saved by older versions", () => {
  // the old date picker stored the picked local moment as a full UTC instant: the day shown must be
  // the one that was picked, not the first 10 characters (2 May 01:30 in UTC+3 is "2024-05-01T22:30Z")
  inEachTimeZone("show the day that was picked", () => {
    for (const date of picked()) {
      const stored = new Date(
        date.getFullYear(),
        date.getMonth(),
        date.getDate(),
        date.getHours(),
        date.getMinutes(),
      ).toISOString();
      // a value that happens to be exactly midnight UTC is read as a new answer: the same day when
      // the picker's time zone is UTC or east of it
      const expected = /T00:00:00\.000Z$/.test(stored) ? stored.slice(0, 10) : "2024-05-02";
      assert.equal(ymd(dayOfIso(stored)), expected, stored);
    }
  });

  inEachTimeZone("new values keep their day", () => {
    for (const value of [
      "2024-05-02T00:00:00Z",
      "2024-05-02T00:00:00.000Z",
      "2024-05-02T00:00:00",
      "2024-05-02T00:00:00+00:00",
      "2024-05-02",
    ]) {
      assert.equal(ymd(dayOfIso(value)), "2024-05-02", value);
    }
  });

  test("not a date", () => {
    assert.equal(dayOfIso("not a date"), null);
    assert.equal(dayOfIso(""), null);
  });
});
