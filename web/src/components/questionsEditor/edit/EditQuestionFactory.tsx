import { BaseQuestion, DateQuestion, MultiSelectQuestion, NumberQuestion, QuestionType, RatingQuestion, SingleSelectAnswer, SingleSelectQuestion, TextQuestion } from "@/common/types";
import EditDateQuestion from "./EditDateQuestion";
import EditMultiSelectQuestion from "./EditMultiSelectQuestion";
import EditNumberQuestion from "./EditNumberQuestion";
import EditRatingQuestion from "./EditRatingQuestion";
import EditSingleSelectQuestion from "./EditSingleSelectQuestion";
import EditTextQuestion from "./EditTextQuestion";
import { MoveDirection } from "../QuestionsEdit";
import { Draggable } from "react-beautiful-dnd";
import { cn } from "@/lib/utils";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { useTranslation } from "react-i18next";
import QuestionActions from "./QuestionActions";

interface EditQuestionFactoryProps {
  languageCode: string;
  questionIdx: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  isInValid: boolean;
  question: BaseQuestion;
  setActiveQuestionId: (questionId: string | undefined) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
}

export default function EditQuestionFactory({
  languageCode,
  questionIdx,
  activeQuestionId,
  isLastQuestion,
  isInValid,
  question,
  setActiveQuestionId,
  moveQuestion,
  updateQuestion,
  duplicateQuestion,
  deleteQuestion,
}: EditQuestionFactoryProps) {

  const { t } = useTranslation();
  const open = activeQuestionId === question.id;

  function getQuestionTypeName(questionType: QuestionType): string {
    switch (questionType) {
      case QuestionType.TextQuestionType: return t("questionEditor.questionType.textQuestion");
      case QuestionType.NumberQuestionType: return t("questionEditor.questionType.numberQuestion");
      case QuestionType.DateQuestionType: return t("questionEditor.questionType.dateQuestion");
      case QuestionType.SingleSelectQuestionType: return t("questionEditor.questionType.singleSelectQuestion");
      case QuestionType.MultiSelectQuestionType: return t("questionEditor.questionType.multiSelectQuestion");
      case QuestionType.RatingQuestionType: return t("questionEditor.questionType.ratingQuestion");
      default: "Unknown"
    }
    return "";
  }

  return (
    <Draggable draggableId={question.id} index={questionIdx}>
      {(provided) => (
        <div
          className={cn(
            open ? "scale-100 shadow-lg" : "scale-97 shadow-md",
            "flex flex-row rounded-lg bg-white transition-all duration-300 ease-in-out"
          )}
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}>
          <div
            className={cn(
              open ? "bg-slate-700" : "bg-slate-400",
              "top-0 w-10 rounded-l-lg p-2 text-center text-sm text-white hover:bg-slate-600",
              isInValid && "bg-red-400  hover:bg-red-600"
            )}>
            {questionIdx + 1}
          </div>
          <Collapsible
            open={open}
            onOpenChange={() => {
              if (activeQuestionId !== question.id) {
                setActiveQuestionId(question.id);
              } else {
                setActiveQuestionId(undefined);
              }
            }}
            className="flex-1 rounded-r-lg border border-slate-200">
            <CollapsibleTrigger
              asChild
              className={cn(open ? "" : "  ", "flex cursor-pointer justify-between p-4 hover:bg-slate-50")}>
              <div>
                <div className="inline-flex">

                  <div>
                    <p className="text-sm font-semibold">
                      {question.code} - {question.text[languageCode] || getQuestionTypeName(question.$questionType)}
                    </p>
                  </div>
                </div>

                <div className="flex items-center space-x-2">
                  <QuestionActions
                    questionIdx={questionIdx}
                    isLastQuestion={isLastQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    moveQuestion={moveQuestion}
                  />
                </div>
              </div>
            </CollapsibleTrigger>
            <CollapsibleContent className="px-4 pb-4">
              {question.$questionType === QuestionType.TextQuestionType ? (
                <EditTextQuestion
                  languageCode={languageCode}
                  question={question as TextQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.DateQuestionType ? (
                <EditDateQuestion
                  languageCode={languageCode}
                  question={question as DateQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.NumberQuestionType ? (
                <EditNumberQuestion
                  languageCode={languageCode}
                  question={question as NumberQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.MultiSelectQuestionType ? (
                <EditMultiSelectQuestion
                  languageCode={languageCode}
                  question={question as MultiSelectQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.SingleSelectQuestionType ? (
                <EditSingleSelectQuestion
                  languageCode={languageCode}
                  question={question as SingleSelectQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid} />
              ) : question.$questionType === QuestionType.RatingQuestionType ? (
                <EditRatingQuestion
                  languageCode={languageCode}
                  question={question as RatingQuestion}
                  questionIdx={questionIdx}
                  moveQuestion={moveQuestion}
                  updateQuestion={updateQuestion}
                  duplicateQuestion={duplicateQuestion}
                  deleteQuestion={deleteQuestion}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  isLastQuestion={isLastQuestion}
                  isInValid={isInValid}
                />
              ) : null}
            </CollapsibleContent>
          </Collapsible>
        </div>
      )}
    </Draggable>
  );
}