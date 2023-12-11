import { TResponseData } from "@/redux/api/responses";
import { TFormQuestion, TFormQuestionType } from "@/redux/api/types";
import OpenTextQuestion from "./questions/OpenTextQuestion";
import MultipleChoiceSingleQuestion from "./questions/MultipleChoiceSingleQuestion";
import MultipleChoiceMultiQuestion from "./questions/MultipleChoiceMultiQuestion";
import RatingQuestion from "./questions/RatingQuestion";

interface QuestionConditionalProps {
  question: TFormQuestion;
  value: string | number | string[];
  onChange: (responseData: TResponseData) => void;
  onSubmit: (data: TResponseData) => void;
  onBack: () => void;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  autoFocus?: boolean;
  formId: string;
}

export default function QuestionConditional({
  question,
  value,
  onChange,
  onSubmit,
  onBack,
  isFirstQuestion,
  isLastQuestion,
  autoFocus = true,
  formId,
}: QuestionConditionalProps) {
  return question.type === TFormQuestionType.OpenText ? (
    <OpenTextQuestion
      question={question}
      value={value}
      onChange={onChange}
      onSubmit={onSubmit}
      onBack={onBack}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
      autoFocus={autoFocus}
    />
  ) : question.type === TFormQuestionType.MultipleChoiceSingle ? (
    <MultipleChoiceSingleQuestion
      question={question}
      value={value}
      onChange={onChange}
      onSubmit={onSubmit}
      onBack={onBack}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
    />
  ) : question.type === TFormQuestionType.MultipleChoiceMulti ? (
    <MultipleChoiceMultiQuestion
      question={question}
      value={value}
      onChange={onChange}
      onSubmit={onSubmit}
      onBack={onBack}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
    />
  )  : question.type === TFormQuestionType.Rating ? (
    <RatingQuestion
      question={question}
      value={value}
      onChange={onChange}
      onSubmit={onSubmit}
      onBack={onBack}
      isFirstQuestion={isFirstQuestion}
      isLastQuestion={isLastQuestion}
    />
  ) : null;
}
