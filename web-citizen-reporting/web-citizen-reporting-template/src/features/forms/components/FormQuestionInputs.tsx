import {
  QuestionType,
  type NumberAnswer,
  type TextAnswer,
} from "@/common/types";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { NumberInput } from "@/components/ui/number-input";
import { useFormContext } from "react-hook-form";
import { mapFormDataToAnswer } from "../utils";

export const FormQuestionNumberInput = ({
  questionId,
  isRequired,
}: {
  questionId: string;
  isRequired?: boolean;
}) => {
  const { control } = useFormContext();
  return (
    <FormField
      control={control}
      name={`question-${questionId}`}
      rules={{
        required: isRequired,
        validate: (value: NumberAnswer | undefined) =>
          value?.value !== undefined,
      }}
      render={({ field }) => (
        <FormItem>
          <FormControl>
            <NumberInput
              onChange={(value) =>
                field.onChange(
                  mapFormDataToAnswer(
                    QuestionType.NumberQuestionType,
                    questionId,
                    value
                  )
                )
              }
              onBlur={field.onBlur}
              value={(field?.value as NumberAnswer)?.value}
            />
          </FormControl>

          <FormMessage />
        </FormItem>
      )}
    />
  );
};

export const FormQuestionTextInput = ({
  questionId,
  isRequired,
}: {
  questionId: string;
  isRequired?: boolean;
}) => {
  const { control } = useFormContext();
  return (
    <FormField
      control={control}
      name={`question-${questionId}`}
      rules={{
        required: isRequired,
        validate: (value: TextAnswer | undefined) => value?.text !== "",
      }}
      render={({ field }) => (
        <FormItem>
          <FormControl>
            <Input
              onChange={(evemt) =>
                field.onChange(
                  mapFormDataToAnswer(
                    QuestionType.TextQuestionType,
                    questionId,
                    evemt.target.value
                  )
                )
              }
              onBlur={field.onBlur}
              value={(field?.value as TextAnswer)?.text}
            />
          </FormControl>

          <FormMessage />
        </FormItem>
      )}
    />
  );
};
