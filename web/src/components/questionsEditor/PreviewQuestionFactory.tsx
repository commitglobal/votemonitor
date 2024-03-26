import { BaseAnswer, BaseQuestion, DateAnswer, DateQuestion, MultiSelectAnswer, MultiSelectQuestion, NumberAnswer, NumberQuestion, QuestionType, RatingAnswer, RatingQuestion, SingleSelectAnswer, SingleSelectQuestion, TextAnswer, TextQuestion } from "@/common/types";
import PreviewDateQuestion from "./PreviewDateQuestion";
import PreviewMultiSelectQuestion from "./PreviewMultiSelectQuestion";
import PreviewNumberQuestion from "./PreviewNumberQuestion";
import PreviewRatingQuestion from "./PreviewRatingQuestion";
import PreviewSingleSelectQuestion from "./PreviewSingleSelectQuestion";
import PreviewTextQuestion from "./PreviewTextQuestion";

interface PreviewQuestionFactoryProps {
  languageCode: string;
  question: BaseQuestion;
  answer: BaseAnswer;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  onSubmitAnswer: (answer: BaseAnswer) => void;
  onBackButtonClicked: () => void;
}

export default function PreviewQuestionFactory({
  languageCode,
  question,
  answer,
  isFirstQuestion,
  isLastQuestion,
  onSubmitAnswer,
  onBackButtonClicked
}: PreviewQuestionFactoryProps) {
  return question.$questionType === QuestionType.TextQuestionType ? (
    <PreviewTextQuestion
      languageCode={languageCode}
      question={question as TextQuestion}
      answer={answer as TextAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked}
    />
  ) : question.$questionType === QuestionType.DateQuestionType ? (
    <PreviewDateQuestion
      languageCode={languageCode}
      question={question as DateQuestion}
      answer={answer as DateAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked}
    />
  ) : question.$questionType === QuestionType.NumberQuestionType ? (
    <PreviewNumberQuestion
      languageCode={languageCode}
      question={question as NumberQuestion}
      answer={answer as NumberAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked}
    />
  ) : question.$questionType === QuestionType.MultiSelectQuestionType ? (
    <PreviewMultiSelectQuestion
      languageCode={languageCode}
      question={question as MultiSelectQuestion}
      answer={answer as MultiSelectAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked}
    />
  ) : question.$questionType === QuestionType.SingleSelectQuestionType ? (
    <PreviewSingleSelectQuestion
      languageCode={languageCode}
      question={question as SingleSelectQuestion}
      answer={answer as SingleSelectAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked} />
  ) : question.$questionType === QuestionType.RatingQuestionType ? (
    <PreviewRatingQuestion
      languageCode={languageCode}
      question={question as RatingQuestion}
      answer={answer as RatingAnswer}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      onSubmitAnswer={onSubmitAnswer}
      onBackButtonClicked={onBackButtonClicked}
    />
  ) : null;
}