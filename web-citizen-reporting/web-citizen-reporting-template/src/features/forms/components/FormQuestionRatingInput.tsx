import {
  QuestionType,
  type NumberAnswer,
  type RatingAnswer,
  type RatingQuestion,
} from "@/common/types";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { RatingGroup } from "@/components/ui/ratings";
import { useFormContext } from "react-hook-form";
import { mapFormDataToAnswer, ratingScaleToNumber } from "../utils";

export const FormQuestionRatingInput = ({
  question,
  language,
  isRequired,
}: {
  question: RatingQuestion;
  language: string;
  isRequired?: boolean;
}) => {
  const { control } = useFormContext();
  return (
    <FormField
      control={control}
      name={`question-${question.id}`}
      rules={{
        required: isRequired,
        validate: (value: NumberAnswer | undefined) =>
          value?.value !== undefined,
      }}
      render={({ field }) => (
        <FormItem>
          <FormControl>
            <RatingGroup
              scale={ratingScaleToNumber(question.scale)}
              lowerLabel={question.lowerLabel?.[language]}
              upperLabel={question.upperLabel?.[language]}
              onValueChange={(value) =>
                field.onChange(
                  mapFormDataToAnswer(
                    QuestionType.RatingQuestionType,
                    question.id,
                    value
                  )
                )
              }
              onBlur={field.onBlur}
              value={(field?.value as RatingAnswer)?.value?.toString()}
            />
          </FormControl>

          <FormMessage />
        </FormItem>
      )}
    />
  );
};
