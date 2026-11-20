import { describe, test } from "node:test";
import assert from "node:assert/strict";
import { shouldDisplayQuestion } from "../services/display-logic.ts";
import type { ApiFormAnswer } from "../services/interfaces/answer.type.ts";
import type { ApiFormQuestion } from "../services/interfaces/question.type.ts";

// a question shown when its parent "p" meets the rule
const child = (displayLogic: any) =>
  ({
    id: "c",
    code: "c",
    text: { EN: "c" },
    helptext: {},
    displayLogic,
    $questionType: "textQuestion",
  }) as unknown as ApiFormQuestion;

const answered = (answer: Record<string, unknown>) => ({
  p: { questionId: "p", ...answer } as ApiFormAnswer,
});

const single = (optionId: string) =>
  answered({ $answerType: "singleSelectAnswer", selection: { optionId } });
const multi = (...optionIds: string[]) =>
  answered({
    $answerType: "multiSelectAnswer",
    selection: optionIds.map((optionId) => ({ optionId })),
  });
const number = (value: number) => answered({ $answerType: "numberAnswer", value });
const rating = (value: number) => answered({ $answerType: "ratingAnswer", value });
const text = (value: string) => answered({ $answerType: "textAnswer", text: value });

describe("shouldDisplayQuestion", () => {
  test("a question without display logic is always shown", () => {
    assert.equal(shouldDisplayQuestion(child(null), undefined), true);
    assert.equal(shouldDisplayQuestion(child(null), {}), true);
  });

  test("hidden while its parent has no answer", () => {
    const q = child({ parentQuestionId: "p", condition: "Equals", value: "1" });
    assert.equal(shouldDisplayQuestion(q, {}), false);
    assert.equal(shouldDisplayQuestion(q, undefined), false);
  });

  test("hidden when the rule names neither a value nor options", () => {
    const q = child({ parentQuestionId: "p", condition: "AnyOf", value: null, optionIds: [] });
    assert.equal(shouldDisplayQuestion(q, single("yes")), false);
  });

  describe("select parents", () => {
    const anyOf = child({ parentQuestionId: "p", condition: "AnyOf", optionIds: ["a", "b"] });
    const all = child({ parentQuestionId: "p", condition: "All", optionIds: ["a", "b"] });

    test("AnyOf: one of the options picked", () => {
      assert.equal(shouldDisplayQuestion(anyOf, single("b")), true);
      assert.equal(shouldDisplayQuestion(anyOf, single("c")), false);
      assert.equal(shouldDisplayQuestion(anyOf, multi("c", "a")), true);
      assert.equal(shouldDisplayQuestion(anyOf, multi("c")), false);
      assert.equal(shouldDisplayQuestion(anyOf, multi()), false);
    });

    test("All: every option picked", () => {
      assert.equal(shouldDisplayQuestion(all, multi("a", "b", "c")), true);
      assert.equal(shouldDisplayQuestion(all, multi("a")), false);
      assert.equal(shouldDisplayQuestion(all, single("a")), false);
    });

    test("All with no options is never met", () => {
      const q = child({ parentQuestionId: "p", condition: "All", value: "x", optionIds: [] });
      assert.equal(shouldDisplayQuestion(q, multi("x")), false);
    });

    test("an older rule naming one option in value", () => {
      const q = child({ parentQuestionId: "p", condition: "Equals", value: "a" });
      assert.equal(shouldDisplayQuestion(q, single("a")), true);
      assert.equal(shouldDisplayQuestion(q, single("b")), false);
      assert.equal(shouldDisplayQuestion(q, multi("b", "a")), true);
    });
  });

  describe("number and rating parents compare as numbers", () => {
    const rule = (condition: string, value: string) =>
      child({ parentQuestionId: "p", condition, value });

    test("each condition", () => {
      assert.equal(shouldDisplayQuestion(rule("Equals", "5"), number(5)), true);
      assert.equal(shouldDisplayQuestion(rule("Equals", "5"), number(4)), false);
      assert.equal(shouldDisplayQuestion(rule("NotEquals", "5"), number(4)), true);
      assert.equal(shouldDisplayQuestion(rule("GreaterThan", "5"), number(6)), true);
      assert.equal(shouldDisplayQuestion(rule("GreaterThan", "5"), number(5)), false);
      assert.equal(shouldDisplayQuestion(rule("GreaterEqual", "5"), number(5)), true);
      assert.equal(shouldDisplayQuestion(rule("LessThan", "5"), number(4)), true);
      assert.equal(shouldDisplayQuestion(rule("LessThan", "5"), number(5)), false);
      assert.equal(shouldDisplayQuestion(rule("LessEqual", "5"), number(5)), true);
    });

    test("10 is greater than 9 (not compared as text)", () => {
      assert.equal(shouldDisplayQuestion(rule("GreaterThan", "9"), number(10)), true);
      assert.equal(shouldDisplayQuestion(rule("GreaterThan", "9"), rating(10)), true);
    });

    test("0 is an answer", () => {
      assert.equal(shouldDisplayQuestion(rule("Equals", "0"), number(0)), true);
    });

    test("a value that isn't a number is never met", () => {
      assert.equal(shouldDisplayQuestion(rule("NotEquals", "abc"), number(3)), false);
    });

    test("ratings", () => {
      assert.equal(shouldDisplayQuestion(rule("LessEqual", "2"), rating(2)), true);
      assert.equal(shouldDisplayQuestion(rule("LessEqual", "2"), rating(3)), false);
    });
  });

  test("text parents compare as text", () => {
    const equals = child({ parentQuestionId: "p", condition: "Equals", value: "yes" });
    const notEquals = child({ parentQuestionId: "p", condition: "NotEquals", value: "yes" });
    assert.equal(shouldDisplayQuestion(equals, text("yes")), true);
    assert.equal(shouldDisplayQuestion(equals, text("no")), false);
    assert.equal(shouldDisplayQuestion(notEquals, text("no")), true);
  });

  test("an unknown condition on a number parent is never met", () => {
    const q = child({ parentQuestionId: "p", condition: "Between", value: "5" });
    assert.equal(shouldDisplayQuestion(q, number(5)), false);
  });

  test("a parent answer type that can't drive display logic hides the question", () => {
    const q = child({ parentQuestionId: "p", condition: "Equals", value: "2024-05-02" });
    const answers = answered({ $answerType: "dateAnswer", date: "2024-05-02" });
    assert.equal(shouldDisplayQuestion(q, answers), false);
  });
});
