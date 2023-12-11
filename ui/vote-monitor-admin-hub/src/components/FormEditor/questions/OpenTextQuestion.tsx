import { cn } from "@/lib/utils";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { TResponseData } from "@/redux/api/responses";
import { TFormMultipleChoiceMultiQuestion, TFormOpenTextQuestion } from "@/redux/api/types";
import Headline from "./Headline";
import Subheader from "./Subheader";
import { BackButton } from "./BackButton";
import SubmitButton from "./SubmitButton";


interface OpenTextQuestionProps {
  question: TFormOpenTextQuestion;
  value: string | number | string[];
  onChange: (responseData: TResponseData) => void;
  onSubmit: (data: TResponseData) => void;
  onBack: () => void;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  autoFocus?: boolean;
}

export default function OpenTextQuestion({
  question,
  value,
  onChange,
  onSubmit,
  onBack,
  isFirstQuestion,
  isLastQuestion,
  autoFocus = true,
}: OpenTextQuestionProps) {
  const handleInputChange = (inputValue: string) => {
    // const isValidInput = validateInput(inputValue, question.inputType, question.required);
    // setIsValid(isValidInput);
    onChange({ [question.id]: inputValue });
  };
  const openTextRef = useCallback(
    (currentElement: HTMLInputElement | HTMLTextAreaElement | null) => {
      if (currentElement && autoFocus) {
        currentElement.focus();
      }
    },
    [question.id]
  );
  const isInputEmpty = (value: string) => {
    return !value?.trim();
  };

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        //  if ( validateInput(value as string, question.inputType, question.required)) {
        onSubmit({ [question.id]: value, inputType: question.inputType });
        // }
      }}
      className="w-full">
      <Headline headline={question.headline} questionId={question.id} />
      <Subheader subheader={question.subheader} questionId={question.id} />
      <div className="mt-4">
        {question.longAnswer === false ? (
          <input
            ref={openTextRef}
            tabIndex={1}
            name={question.id}
            id={question.id}
            placeholder={question.placeholder}
            required={true}
            value={value ? (value as string) : ""}
            type={question.inputType}
            onInput={(e) => handleInputChange(e.currentTarget.value)}
            autoFocus={autoFocus}
            className="border-border focus:border-border-highlight block w-full rounded-md border p-2 shadow-sm focus:outline-none focus:ring-0 sm:text-sm"
            onKeyDown={(e) => {
              if (e.key === "Enter" && isInputEmpty(value as string)) {
                e.preventDefault(); // Prevent form submission
              } else if (e.key === "Enter") {
                onSubmit({ [question.id]: value, inputType: question.inputType });
              }
            }}
          />
        ) : (
          <textarea
            ref={openTextRef}
            rows={3}
            name={question.id}
            tabIndex={1}
            id={question.id}
            placeholder={question.placeholder}
            required={true}
            value={value as string}
            onInput={(e) => handleInputChange(e.currentTarget.value)}
            autoFocus={autoFocus}
            className="border-border text-subheading focus:border-border-highlight block w-full rounded-md border p-2 shadow-sm focus:ring-0 sm:text-sm"
          />
        )}
      </div>

      <div className="mt-4 flex w-full justify-between">
        {!isFirstQuestion && (
          <BackButton
            backButtonLabel={"Back"}
            onClick={() => {
              onBack();
            }}
          />
        )}
        <div></div>
        <SubmitButton isLastQuestion={isLastQuestion} onClick={() => { }} />
      </div>
    </form>
  );
}
