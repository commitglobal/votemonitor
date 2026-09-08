import {
  AnswerType,
  type MultiSelectAnswer,
  type MultiSelectQuestion,
  type SelectedOption,
} from "@/common/types";
import { Checkbox } from "@/components/ui/checkbox";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useFormContext } from "react-hook-form";
import { useFreeTextInput } from "../hooks/useFreeTextInput";
import { FormQuestionFreeTextInput } from "./FormQuestionFreeTextInput";

export const FormQuestionMultiSelectInput = ({
  question,
  language,
  isRequired,
}: {
  question: MultiSelectQuestion;
  language: string;
  isRequired?: boolean;
}) => {
  const { control } = useFormContext();

  const { fieldNames, shouldDisplayFreeTextInput } = useFreeTextInput(question);

  const addOptionToMultiSelectAnswer = (
    questionId: string,
    currentValue: MultiSelectAnswer,
    option: SelectedOption
  ) => {
    let selections = currentValue?.selection ?? [];
    selections = [...selections, option];
    let multiselectAnswer: MultiSelectAnswer = {
      $answerType: AnswerType.MultiSelectAnswerType,
      questionId,
      selection: selections,
    };

    return multiselectAnswer;
  };
  const removeSelectionFromMultiSelectAnswer = (
    questionId: string,
    currentValue: MultiSelectAnswer,
    optionId: string
  ) => {
    let selections = currentValue?.selection ?? [];
    const filteredSelections = selections.filter(
      (selected) => selected.optionId !== optionId
    );

    const multiselectAnswer: MultiSelectAnswer = {
      $answerType: AnswerType.MultiSelectAnswerType,
      questionId,
      selection: filteredSelections,
    };
    return multiselectAnswer;
  };

  return (
    <div className="flex flex-col gap-4">
      <FormField
        control={control}
        name={fieldNames.parent}
        rules={{
          required: isRequired,
          validate: (value: MultiSelectAnswer | undefined) =>
            value?.selection && value.selection.length > 0,
        }}
        render={({ field }) => (
          <FormItem>
            {question.options.map((option) => {
              const isChecked = field.value?.selection?.some(
                (opt: SelectedOption) => opt.optionId === option.id
              );

              return (
                <FormItem
                  key={option.id}
                  className="flex flex-row items-start space-x-3 space-y-0"
                >
                  <FormControl>
                    <Checkbox
                      checked={isChecked}
                      onCheckedChange={(checked) => {
                        const newValue = checked
                          ? addOptionToMultiSelectAnswer(
                              question.id,
                              field.value,
                              {
                                optionId: option.id,
                              }
                            )
                          : removeSelectionFromMultiSelectAnswer(
                              question.id,
                              field.value,
                              option.id
                            );
                        field.onChange(newValue);
                      }}
                    />
                  </FormControl>
                  <FormLabel className="text-sm font-normal">
                    {option.text[language]}
                  </FormLabel>
                </FormItem>
              );
            })}
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
