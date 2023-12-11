import { useState } from "react";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { FormModel, TFormQuestionType } from "@/redux/api/types";
import { Draggable } from "react-beautiful-dnd";
import { cn } from "@/lib/utils";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "../ui/collapsible";
import { Switch } from "../ui/switch";
import { getTFormQuestionTypeName } from "./questions";
import OpenQuestionForm from "./OpenQuestionForm";
import AdvancedSettings from "./AdvancedSettings";
import QuestionActions from "./QuestionMenu";
import MultipleChoiceSingleForm from "./MultipleChoiceSingleForm";
import MultipleChoiceMultiForm from "./MultipleChoiceMultiForm";
import RatingQuestionForm from "./RatingQuestionForm";

interface QuestionCardProps {
  localForm: FormModel;
  questionIdx: number;
  moveQuestion: (questionIndex: number, up: boolean) => void;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
  deleteQuestion: (questionIdx: number) => void;
  duplicateQuestion: (questionIdx: number) => void;
  activeQuestionId: string | null;
  setActiveQuestionId: (questionId: string | null) => void;
  lastQuestion: boolean;
  isInValid: boolean;
}

export default function QuestionCard({
  localForm,
  questionIdx,
  moveQuestion,
  updateQuestion,
  duplicateQuestion,
  deleteQuestion,
  activeQuestionId,
  setActiveQuestionId,
  lastQuestion,
  isInValid,
}: QuestionCardProps) {
  const question = localForm.questions[questionIdx];
  const open = activeQuestionId === question.id;
  const [openAdvanced, setOpenAdvanced] = useState(question.logic && question.logic.length > 0);

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
                setActiveQuestionId(null);
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
                      {question.id} - {question.headline || getTFormQuestionTypeName(question.type)}
                    </p>
                  </div>
                </div>

                <div className="flex items-center space-x-2">
                  <QuestionActions
                    questionIdx={questionIdx}
                    lastQuestion={lastQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    moveQuestion={moveQuestion}
                  />
                </div>
              </div>
            </CollapsibleTrigger>
            <CollapsibleContent className="px-4 pb-4">
              {question.type === TFormQuestionType.OpenText ? (
                <OpenQuestionForm
                  localForm={localForm}
                  question={question}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.type === TFormQuestionType.MultipleChoiceSingle ? (
                <MultipleChoiceSingleForm
                  localForm={localForm}
                  question={question}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.type === TFormQuestionType.MultipleChoiceMulti ? (
                <MultipleChoiceMultiForm
                  localForm={localForm}
                  question={question}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.type === TFormQuestionType.Rating ? (
                <RatingQuestionForm
                  localForm={localForm}
                  question={question}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : null}

              <div className="mt-4">
                <Collapsible open={openAdvanced} onOpenChange={setOpenAdvanced} className="mt-5">
                  <CollapsibleTrigger className="flex items-center text-xs text-slate-700">
                    {openAdvanced ? "Hide Advanced Settings" : "Show Advanced Settings"}
                  </CollapsibleTrigger>

                  <CollapsibleContent className="space-y-4">
                    <AdvancedSettings
                      question={question}
                      questionIdx={questionIdx}
                      localForm={localForm}
                      updateQuestion={updateQuestion}
                    />
                  </CollapsibleContent>
                </Collapsible>
              </div>
            </CollapsibleContent>

            {open && (
              <div className="mx-4 flex justify-end space-x-6 border-t border-slate-200">
                {question.type === "openText" && (
                  <div className="my-4 flex items-center justify-end space-x-2">
                    <Label htmlFor="longAnswer">Long Answer</Label>
                    <Switch
                      id="longAnswer"
                      checked={question.longAnswer !== false}
                      onClick={(e) => {
                        e.stopPropagation();
                        updateQuestion(questionIdx, {
                          longAnswer:
                            typeof question.longAnswer === "undefined" ? false : !question.longAnswer,
                        });
                      }}
                    />
                  </div>
                )}
              </div>
            )}
          </Collapsible>
        </div>
      )}
    </Draggable>
  );
}
