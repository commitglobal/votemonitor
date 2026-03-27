import {
  type MultiSelectQuestion,
  type SingleSelectQuestion,
} from "@/common/types";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Textarea } from "@/components/ui/textarea";
import { useFormContext } from "react-hook-form";
import { useFreeTextInput } from "../hooks/useFreeTextInput";
export const FormQuestionFreeTextInput = ({
  question,
  language,
}: {
  question: SingleSelectQuestion | MultiSelectQuestion;
  language: string;
}) => {
  const { control, setValue } = useFormContext();
  const {
    fieldNames,
    shouldDisplayFreeTextInput,
    freeTextOption,
    addFreeTextToOption,
  } = useFreeTextInput(question);
  return (
    <FormField
      control={control}
      name={fieldNames.freeText}
      rules={{
        required: shouldDisplayFreeTextInput,
      }}
      render={({ field }) => (
        <FormItem>
          <FormControl>
            <Textarea
              {...field}
              onChange={(event) => {
                const updatedData = addFreeTextToOption(event.target.value);
                setValue(fieldNames.parent, updatedData);
                field.onChange(event);
              }}
              placeholder={freeTextOption?.text?.[language]}
            />
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  );
};
