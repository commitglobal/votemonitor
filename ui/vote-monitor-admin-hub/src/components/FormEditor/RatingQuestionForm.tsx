
import { ChevronDownIcon, PlusIcon, TrashIcon } from "@heroicons/react/24/solid";
import { useMemo, useState } from "react";
import { FormModel, TFormRatingQuestion } from "@/redux/api/types";
import QuestionFormInput from "./QuestionFormInput";
import { Label } from "../ui/label";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuPortal, DropdownMenuTrigger } from "../ui/dropdown-menu";

interface RatingQuestionFormProps {
  localForm: FormModel;
  question: TFormRatingQuestion;
  questionIdx: number;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
  isInValid: boolean;
}

type Option = {
  label: string;
  value: number;
};

export default function RatingQuestionForm({
  question,
  questionIdx,
  updateQuestion,
  isInValid,
  localForm,
}: RatingQuestionFormProps) {
  const [showSubheader, setShowSubheader] = useState(!!question.subheader);
  const options = useMemo(() => [
    { label: "5 points (recommended)", value: 5 },
    { label: "3 points", value: 3 },
    { label: "4 points", value: 4 },
    { label: "7 points", value: 7 },
    { label: "10 points", value: 10 },
  ], []);
  const [selectedOption, setSelectedOption] = useState<Option>(options[0]);


  const handleSelect = (option: Option) => {
    setSelectedOption(option);
    updateQuestion(questionIdx, { range: option.value })
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
          <Button size="sm" variant="outline" type="button" onClick={() => setShowSubheader(true)}>
            <PlusIcon className="mr-1 h-4 w-4" />
            Add Description
          </Button>
        )}
      </div>

      <div className="mt-3 flex justify-between gap-8">
        <div className="flex-1">
          <Label htmlFor="subheader">Range</Label>
          <div className="mt-2">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <button
                  type="button"
                  className="flex h-10 w-full rounded-md border border-slate-300 bg-transparent px-3 py-2 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none disabled:cursor-not-allowed disabled:opacity-50 dark:border-slate-500 dark:text-slate-300">
                  <span className="flex flex-1">
                    <span>{selectedOption ? selectedOption.label : "Select an option"}</span>
                  </span>
                  <span className="flex h-full items-center border-l pl-3">
                    <ChevronDownIcon className="h-4 w-4 text-gray-500" />
                  </span>
                </button>
              </DropdownMenuTrigger>

              <DropdownMenuPortal>
                <DropdownMenuContent
                  className="min-w-[220px] rounded-md bg-white text-sm text-slate-800 shadow-md"
                  align="start">
                  {options.map((option) => (
                    <DropdownMenuItem
                      key={option.value}
                      className="flex cursor-pointer items-center p-3 hover:bg-gray-100 hover:outline-none data-[disabled]:cursor-default data-[disabled]:opacity-50"
                      onSelect={() => handleSelect(option)}>
                      {option.label}
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuContent>
              </DropdownMenuPortal>
            </DropdownMenu>
          </div>
        </div>
      </div>

      <div className="mt-3 flex justify-between gap-8">
        <div className="flex-1">
          <Label htmlFor="lowerLabel">Lower label</Label>
          <div className="mt-2">
            <Input
              id="lowerLabel"
              name="lowerLabel"
              placeholder="Not good"
              value={question.lowerLabel}
              onChange={(e) => updateQuestion(questionIdx, { lowerLabel: e.target.value })}
            />
          </div>
        </div>
        <div className="flex-1">
          <Label htmlFor="upperLabel">Upper label</Label>
          <div className="mt-2">
            <Input
              id="upperLabel"
              name="upperLabel"
              placeholder="Very satisfied"
              value={question.upperLabel}
              onChange={(e) => updateQuestion(questionIdx, { upperLabel: e.target.value })}
            />
          </div>
        </div>
      </div>
    </form>
  );
}
