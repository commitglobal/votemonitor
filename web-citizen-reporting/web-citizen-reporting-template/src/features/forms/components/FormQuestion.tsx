import { type BaseQuestion } from "@/common/types";
import { FormDescription, FormLabel } from "@/components/ui/form";
import { currentFormLanguageAtom } from "@/features/forms/atoms";
import {
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from "@/lib/utils";
import { useAtomValue } from "jotai";
import { useEffect } from "react";
import { useFormContext } from "react-hook-form";
import { useShouldDisplayQuestion } from "../hooks/useShouldDisplayQuestion";
import {
  FormQuestionNumberInput,
  FormQuestionTextInput,
} from "./FormQuestionInputs";
import { FormQuestionMultiSelectInput } from "./FormQuestionMultiSelectInput";
import { FormQuestionSingleSelectInput } from "./FormQuestionSingleSelectInput";
import { FormQuestionRatingInput } from "./FormQuestionRatingInput";

interface FormQuestionProps {
  question: BaseQuestion;
  isRequired: boolean;
}

const BaseFormQuestion = ({ question, isRequired }: FormQuestionProps) => {
  const language = useAtomValue(currentFormLanguageAtom);

  return (
    <div className="w-full flex flex-col gap-4" key={question.id}>
      <div>
        <FormLabel>
          <span> {`${question.code} - ${question.text[language]}`}</span>
        </FormLabel>

        <FormDescription>{question?.helptext?.[language]}</FormDescription>
      </div>
      {isNumberQuestion(question) && (
        <FormQuestionNumberInput
          questionId={question.id}
          isRequired={isRequired}
        />
      )}
      {isTextQuestion(question) && (
        <FormQuestionTextInput
          questionId={question.id}
          isRequired={isRequired}
        />
      )}
      {isSingleSelectQuestion(question) && (
        <FormQuestionSingleSelectInput
          question={question}
          language={language}
          isRequired={isRequired}
        />
      )}
      {isMultiSelectQuestion(question) && (
        <FormQuestionMultiSelectInput
          question={question}
          language={language}
          isRequired={isRequired}
        />
      )}

      {isRatingQuestion(question) && (
        <FormQuestionRatingInput
          question={question}
          language={language}
          isRequired={isRequired}
        />
      )}
    </div>
  );
};

const QuestionWithDisplayLogic = ({ question }: { question: BaseQuestion }) => {
  const { control, unregister } = useFormContext();
  const shouldDisplay = useShouldDisplayQuestion({ question, control });

  useEffect(() => {
    const fieldName = `question-${question.id}`;
    return () => {
      // unregister field if user changes the value of the parent and the condition stops matching
      if (shouldDisplay) unregister(fieldName);
    };
  }, [shouldDisplay]);

  if (!shouldDisplay) return;
  return <BaseFormQuestion question={question} isRequired={shouldDisplay} />;
};

export const FormQuestion = ({ question }: { question: BaseQuestion }) => {
  if (question.displayLogic)
    return <QuestionWithDisplayLogic question={question} />;
  return <BaseFormQuestion question={question} isRequired={true} />;
};
