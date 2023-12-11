import { FormModel, TFormQuestion } from "@/redux/api/types";
import { RefObject, useState } from "react";
import { Label } from "../ui/label";
import { Input } from "../ui/input";
import { toast } from "react-toastify";

interface QuestionFormInputProps {
  localForm: FormModel;
  question: TFormQuestion;
  questionIdx: number;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
  isInValid: boolean;
  ref?: RefObject<HTMLInputElement>;
}

const QuestionFormInput = ({
  localForm,
  question,
  questionIdx,
  updateQuestion,
  isInValid,
  ref,
}: QuestionFormInputProps) => {
  return (
    <>
      <div className="mt-3">
        <Label htmlFor="questionCode">Question Code</Label>
        <div className="mt-2 flex flex-col gap-6">
          <div className="flex items-center space-x-2">
          <Input
              autoFocus
              ref={ref}
              id="questionCode"
              name="questionCode"
              value={question.code}
              onChange={(e) => updateQuestion(questionIdx, { code: e.target.value })}
              className={isInValid && question.code.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
            />
          </div>
        </div>
      </div>
      <div className="mt-3">
        <Label htmlFor="headline">Question</Label>
        <div className="mt-2 flex flex-col gap-6">
          <div className="flex items-center space-x-2">
            <Input
              id="headline"
              name="headline"
              value={question.headline}
              onChange={(e) => updateQuestion(questionIdx, { headline: e.target.value })}
              className={isInValid && question.headline.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default QuestionFormInput;
