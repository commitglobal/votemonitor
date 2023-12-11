import { TFormOpenTextQuestionInputType } from "@/redux/api/types";
import {
  ChatBubbleIcon,
  GridIcon,
} from "@radix-ui/react-icons";
import React from "react";

interface TFormQuestion {
  value: string;
  label: string;
}

interface QuestionTypeSelectorProps {
  questionTypes: TFormQuestion[];
  currentType: string | undefined;
  handleTypeChange: (value: TFormOpenTextQuestionInputType) => void;
}

const typeIcons: { [key: string]: React.ReactNode } = {
  text: <ChatBubbleIcon />,
  number: <GridIcon />,
};

export function QuestionTypeSelector({
  questionTypes,
  currentType,
  handleTypeChange,
}: QuestionTypeSelectorProps): JSX.Element {
  return (
    <div className="flex w-full items-center justify-between rounded-md border p-1">
      {questionTypes.map((type) => (
        <div
          key={type.value}
          onClick={() => handleTypeChange(type.value as TFormOpenTextQuestionInputType)}
          className={`flex-grow cursor-pointer rounded-md bg-${
            (currentType === undefined && type.value === "text") || currentType === type.value
              ? "slate-100"
              : "white"
          } p-2 text-center`}>
          <div className="flex items-center justify-center space-x-2">
            <span className="text-sm text-slate-900">{type.label}</span>
            <div className="h-4 w-4 text-slate-600 hover:text-slate-800">{typeIcons[type.value]}</div>
          </div>
        </div>
      ))}
    </div>
  );
}
