import {
  QuestionType,
  type SingleSelectAnswer,
  type SingleSelectQuestion,
} from "@/common/types";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { useFormContext } from "react-hook-form";
import { useFreeTextInput } from "../hooks/useFreeTextInput";
import { mapFormDataToAnswer } from "../utils";
import { FormQuestionFreeTextInput } from "./FormQuestionFreeTextInput";

export const FormQuestionSingleSelectInput = ({
  question,
  language,
  isRequired,
}: {
  question: SingleSelectQuestion;
  language: string;
  isRequired?: boolean;
}) => {
  const { control } = useFormContext();
  const { fieldNames, shouldDisplayFreeTextInput } = useFreeTextInput(question);

  return (
    <div className="flex flex-col gap-4">
      <FormField
        control={control}
        name={fieldNames.parent}
        rules={{ required: isRequired }}
        render={({ field }) => (
          <FormItem className="space-y-3">
            <FormControl>
              <RadioGroup
                onValueChange={(value) =>
                  field.onChange(
                    mapFormDataToAnswer(
                      QuestionType.SingleSelectQuestionType,
                      question.id,
                      value
                    )
                  )
                }
                value={
                  (field?.value as SingleSelectAnswer)?.selection?.optionId
                }
                className="flex flex-col space-y-1"
              >
                {question.options.map((option) => (
                  <FormItem
                    className="flex items-center space-x-3 space-y-0"
                    key={`${question.id}-${option.id}`}
                  >
                    <FormControl>
                      <RadioGroupItem value={option.id} />
                    </FormControl>
                    <FormLabel className="font-normal">
                      {option.text[language]}
                    </FormLabel>
                  </FormItem>
                ))}
              </RadioGroup>
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
      {shouldDisplayFreeTextInput && (
        <FormQuestionFreeTextInput question={question} language={language} />
      )}
    </div>
  );
};
