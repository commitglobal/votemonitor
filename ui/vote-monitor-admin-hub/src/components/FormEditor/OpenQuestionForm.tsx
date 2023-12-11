"use client";

import {
  FormModel,
  TFormOpenTextQuestion,
  TFormOpenTextQuestionInputType,
} from "@/redux/api/types";
import { PlusIcon, TrashIcon } from "@radix-ui/react-icons";
import { useState } from "react";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Label } from "../ui/label";
import QuestionFormInput from "./QuestionFormInput";
import { QuestionTypeSelector } from "./QuestionTypeSelector";

const questionTypes = [
  { value: "text", label: "Text" },
  { value: "number", label: "Number" },
];

interface OpenQuestionFormProps {
  localForm: FormModel;
  question: TFormOpenTextQuestion;
  questionIdx: number;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
  isInValid: boolean;
}

export default function OpenQuestionForm({
  localForm,
  question,
  questionIdx,
  updateQuestion,
  isInValid,
}: OpenQuestionFormProps): JSX.Element {
  const [showSubheader, setShowSubheader] = useState(!!question.subheader);
  const defaultPlaceholder = getPlaceholderByInputType(question.inputType ?? "text");

  const handleInputChange = (inputType: TFormOpenTextQuestionInputType) => {
    const updatedAttributes = {
      inputType: inputType,
      placeholder: getPlaceholderByInputType(inputType),
      longAnswer: inputType === "text" ? question.longAnswer : false,
    };
    updateQuestion(questionIdx, updatedAttributes);
  };


  return (
    <form>
      <QuestionFormInput
        localForm={localForm}
        isInValid={isInValid}
        question={question}
        questionIdx={questionIdx}
        updateQuestion={updateQuestion}
      />

      <div className="mt-3">
        {showSubheader && (
          <>
            <Label htmlFor="subheader">Description</Label>
            <div className="mt-2 inline-flex w-full items-center">
              <Input
                id="subheader"
                name="subheader"
                value={question.subheader}
                onChange={(e) => updateQuestion(questionIdx, { subheader: e.target.value })}
              />
              <TrashIcon
                className="ml-2 h-4 w-4 cursor-pointer text-slate-400 hover:text-slate-500"
                onClick={() => {
                  setShowSubheader(false);
                  updateQuestion(questionIdx, { subheader: "" });
                }}
              />
            </div>
          </>
        )}
        {!showSubheader && (
          <Button size="sm" variant="ghost" type="button" onClick={() => setShowSubheader(true)}>
            <PlusIcon className="mr-1 h-4 w-4" />
            Add Description
          </Button>
        )}
      </div>

      <div className="mt-3">
        <Label htmlFor="placeholder">Placeholder</Label>
        <div className="mt-2">
          <Input
            id="placeholder"
            name="placeholder"
            value={question.placeholder ?? defaultPlaceholder}
            onChange={(e) => updateQuestion(questionIdx, { placeholder: e.target.value })}
          />
        </div>
      </div>

      {/* Add a dropdown to select the question type */}
      <div className="mt-3">
        <Label htmlFor="questionType">Input Type</Label>
        <div className="mt-2 flex items-center">
          <QuestionTypeSelector
            questionTypes={questionTypes}
            currentType={question.inputType}
            handleTypeChange={handleInputChange} // Use the merged function
          />
        </div>
      </div>
    </form>
  );
}

function getPlaceholderByInputType(inputType: TFormOpenTextQuestionInputType) {
  switch (inputType) {
    case "number":
      return "42";

    default:
      return "Type your answer here...";
  }
}
