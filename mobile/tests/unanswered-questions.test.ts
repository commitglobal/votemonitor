import { describe, test } from "node:test";
import assert from "node:assert/strict";
import {
  answersByQuestionId,
  getFirstUnansweredQuestion,
  getResumeQuestion,
  getUnansweredQuestions,
  isEmptyAnswer,
} from "../services/unanswered-questions.ts";
import type { ApiFormAnswer } from "../services/interfaces/answer.type.ts";
import type { ApiFormQuestion } from "../services/interfaces/question.type.ts";

const question = (id: string, $questionType: string, displayLogic: any = null) =>
  ({
    id,
    code: id,
    text: { EN: id },
    helptext: {},
    displayLogic,
    $questionType,
  }) as ApiFormQuestion;

// q1 text, q2 single select (q3 is shown when option "yes" is picked), q4 date, q5 unknown type
const questions = [
  question("q1", "textQuestion"),
  question("q2", "singleSelectQuestion"),
  question("q3", "numberQuestion", {
    parentQuestionId: "q2",
    condition: "AnyOf",
    optionIds: ["yes"],
  }),
  question("q4", "dateQuestion"),
  question("q5", "drawingQuestion"),
];

const ids = (list: ApiFormQuestion[]) => list.map((q) => q.id);

describe("getUnansweredQuestions", () => {
  test("every shown question without an answer, in form order", () => {
    // q3 is hidden (q2 not answered), q5 can't be shown by this version
    assert.deepEqual(ids(getUnansweredQuestions(questions, {})), ["q1", "q2", "q4"]);
  });

  test("a question shown by display logic counts once its parent shows it", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "yes" } },
    ] as ApiFormAnswer[]);
    assert.deepEqual(ids(getUnansweredQuestions(questions, answers)), ["q3", "q4"]);

    const notShown = answersByQuestionId([
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "no" } },
    ] as ApiFormAnswer[]);
    assert.deepEqual(ids(getUnansweredQuestions(questions, notShown)), ["q1", "q4"]);
  });

  test("an empty answer counts as unanswered", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "   " },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "yes" } },
      { $answerType: "numberAnswer", questionId: "q3", value: 0 },
      { $answerType: "dateAnswer", questionId: "q4", date: "2024-05-02T00:00:00Z" },
    ] as ApiFormAnswer[]);
    assert.deepEqual(ids(getUnansweredQuestions(questions, answers)), ["q1"]);
  });

  test("all answered", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "no" } },
      { $answerType: "dateAnswer", questionId: "q4", date: "2024-05-02T00:00:00Z" },
    ] as ApiFormAnswer[]);
    assert.deepEqual(getUnansweredQuestions(questions, answers), []);
    assert.equal(getFirstUnansweredQuestion(questions, answers), undefined);
  });

  test("no form yet", () => {
    assert.deepEqual(getUnansweredQuestions(undefined, undefined), []);
  });
});

describe("getFirstUnansweredQuestion", () => {
  test("the first one in form order, where the warning leads to", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
    ] as ApiFormAnswer[]);
    assert.equal(getFirstUnansweredQuestion(questions, answers)?.id, "q2");
  });
});

describe("getResumeQuestion", () => {
  test("the first unanswered question, like the warning", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(questions, answers)?.id, "q2");
  });

  test("skips a hidden question with no answer", () => {
    // q3 has no answer, but q2 = "no" hides it
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "no" } },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(questions, answers)?.id, "q4");
  });

  test("a question shown by display logic once its parent shows it", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "yes" } },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(questions, answers)?.id, "q3");
  });

  test("an empty answer counts as unanswered", () => {
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "  " },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(questions, answers)?.id, "q1");
  });

  test("all answered: the last question shown", () => {
    // q5 is last and shown, though it can't be answered here
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "q2", selection: { optionId: "no" } },
      { $answerType: "dateAnswer", questionId: "q4", date: "2024-05-02T00:00:00Z" },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(questions, answers)?.id, "q5");
  });

  test("all answered and the last question hidden: the last one shown before it", () => {
    const form = [
      question("a", "textQuestion"),
      question("b", "singleSelectQuestion"),
      question("c", "textQuestion", { parentQuestionId: "b", condition: "AnyOf", optionIds: ["yes"] }),
    ];
    const answers = answersByQuestionId([
      { $answerType: "textAnswer", questionId: "a", text: "queue" },
      { $answerType: "singleSelectAnswer", questionId: "b", selection: { optionId: "no" } },
    ] as ApiFormAnswer[]);
    assert.equal(getResumeQuestion(form, answers)?.id, "b");
  });

  test("every question hidden: the form's last question", () => {
    const form = [
      question("a", "textQuestion", { parentQuestionId: "x", condition: "Equals", value: "1" }),
      question("b", "textQuestion", { parentQuestionId: "x", condition: "Equals", value: "2" }),
    ];
    assert.equal(getResumeQuestion(form, {})?.id, "b");
  });

  test("a form with no questions, or no form yet", () => {
    assert.equal(getResumeQuestion([], {}), undefined);
    assert.equal(getResumeQuestion(undefined, undefined), undefined);
  });
});

describe("answersByQuestionId", () => {
  test("keys the answers by question id", () => {
    const answers = [
      { $answerType: "textAnswer", questionId: "q1", text: "queue" },
      { $answerType: "numberAnswer", questionId: "q3", value: 4 },
    ] as ApiFormAnswer[];
    assert.deepEqual(answersByQuestionId(answers), { q1: answers[0], q3: answers[1] });
  });

  test("no answers", () => {
    assert.deepEqual(answersByQuestionId(undefined), {});
    assert.deepEqual(answersByQuestionId([]), {});
  });
});

describe("isEmptyAnswer", () => {
  const empty: ApiFormAnswer[] = [
    { $answerType: "textAnswer", questionId: "a", text: "" },
    { $answerType: "numberAnswer", questionId: "a", value: undefined as any },
    { $answerType: "ratingAnswer", questionId: "a", value: null as any },
    { $answerType: "dateAnswer", questionId: "a", date: "" },
    { $answerType: "timeAnswer", questionId: "a", time: "" },
    { $answerType: "dateTimeAnswer", questionId: "a", dateTime: "" },
    { $answerType: "singleSelectAnswer", questionId: "a", selection: { optionId: "" } } as any,
    { $answerType: "multiSelectAnswer", questionId: "a", selection: [] },
  ];
  test("answers that say nothing", () => {
    for (const answer of empty) assert.equal(isEmptyAnswer(answer), true, answer.$answerType);
    assert.equal(isEmptyAnswer(undefined), true);
  });

  test("answers that say something, 0 included", () => {
    const filled: ApiFormAnswer[] = [
      { $answerType: "textAnswer", questionId: "a", text: "x" },
      { $answerType: "numberAnswer", questionId: "a", value: 0 },
      { $answerType: "ratingAnswer", questionId: "a", value: 3 },
      { $answerType: "timeAnswer", questionId: "a", time: "13:05:00" },
      { $answerType: "multiSelectAnswer", questionId: "a", selection: [{ optionId: "o" }] } as any,
    ];
    for (const answer of filled) assert.equal(isEmptyAnswer(answer), false, answer.$answerType);
  });
});
