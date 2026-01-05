import { formEditOpts, questionSchema } from "@/components/form-builder/shared";
import { Button } from "@/components/ui/button";
import {
  FieldDescription,
  FieldGroup,
  FieldLegend,
  FieldSet,
} from "@/components/ui/field";
import { withForm } from "@/hooks/form";
import i18n from "@/i18n";
import { questionsIconMapping } from "@/lib/questions-icons";
import { QuestionType, RatingScaleType } from "@/types/form";
import { PlusIcon } from "lucide-react";
import { useState } from "react";
import z from "zod";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";

export const FormQuestions = withForm({
  ...formEditOpts,
  render: ({ form }) => {
    return (
      <form.AppField name="questions" mode="array">
        {(field) => {
          return (
            <FieldSet className="h-full">
              <FieldLegend>Questions</FieldLegend>
              <FieldDescription>The questions of the form.</FieldDescription>
              <FieldGroup>
                <div className="h-[calc(100vh-24rem)] overflow-y-auto">
                  {field.state.value.map((question, index) => {
                    return (
                      <div key={question.questionId} className="p-4 rounded">
                        <div className="flex flex-col sm:flex-row gap-2">
                          <div className="w-full sm:w-[64px]">
                            <form.AppField
                              key={index}
                              name={`questions[${index}].code`}
                            >
                              {(subField) => {
                                return (
                                  <subField.TextInput
                                    label="Code"
                                    id={`questions-${index}-code`}
                                    type="text"
                                  />
                                );
                              }}
                            </form.AppField>
                          </div>

                          <div className="w-full sm:flex-1">
                            <form.AppField
                              key={index}
                              name={`questions[${index}].text`}
                            >
                              {(subField) => {
                                return (
                                  <subField.TextInput
                                    label="Text"
                                    id={`questions-${index}-text`}
                                    type="text"
                                  />
                                );
                              }}
                            </form.AppField>
                          </div>
                        </div>

                        <form.AppField
                          key={index}
                          name={`questions[${index}].helptext`}
                        >
                          {(subField) => {
                            return (
                              <subField.TextInput
                                label="Helptext"
                                id={`questions-${index}-helptext`}
                                type="text"
                              />
                            );
                          }}
                        </form.AppField>

                        <div className="flex flex-row gap-2">
                          <Button
                            className="mt-2"
                            variant="destructive"
                            type="button"
                            onClick={() => field.removeValue(index)}
                          >
                            Remove
                          </Button>

                          <Button
                            className="mt-2"
                            variant="outline"
                            type="button"
                            onClick={() => field.moveValue(index, index - 1)}
                            disabled={index === 0}
                          >
                            move up
                          </Button>

                          <Button
                            className="mt-2"
                            variant="outline"
                            type="button"
                            onClick={() => field.moveValue(index, index + 1)}
                            disabled={index === field.state.value.length - 1}
                          >
                            move down
                          </Button>
                        </div>
                        {index === field.state.value.length - 1 && (
                          <AddQuestionButton onAddQuestion={(question) => field.insertValue(index + 1, question)} />
                        )}
                      </div>
                    );
                  })}
                  {field.state.value.length === 0 && (
                    <AddQuestionButton onAddQuestion={(question) => field.insertValue(0, question)} />
                  )}
                </div>
              </FieldGroup>
            </FieldSet>
          );
        }}
      </form.AppField>
    );
  },
});

export type QuestionTypeConfig = {
  type: QuestionType;
  label: string;
  icon: any;
  create: () => z.infer<typeof questionSchema>;
};

const questionTypes: QuestionTypeConfig[] = [
  {
    type: QuestionType.TextQuestionType,
    icon: questionsIconMapping[QuestionType.TextQuestionType],  
    label: i18n.t('questionEditor.questionType.textQuestion'),
    create: () => {
      const newTextQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.TextQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        inputPlaceholder: '',
        hasDisplayLogic: false,
        code: ''
      };

      return newTextQuestion;
    },
  },
  {
    type: QuestionType.NumberQuestionType,
    icon: questionsIconMapping[QuestionType.NumberQuestionType],
    label: i18n.t('questionEditor.questionType.numberQuestion'),
    create: () => {
      const newNumberQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.NumberQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        inputPlaceholder: '',
        hasDisplayLogic: false,
        code: ''
      };

      return newNumberQuestion;
    },
  },
  {
    type: QuestionType.DateQuestionType,
    icon: questionsIconMapping[QuestionType.DateQuestionType],
    label: i18n.t('questionEditor.questionType.dateQuestion'),
    create: () => {
      const newDateQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.DateQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        hasDisplayLogic: false,
        code: ''
      };

      return newDateQuestion;
    },
  },
  {
    type: QuestionType.RatingQuestionType,
    icon: questionsIconMapping[QuestionType.RatingQuestionType],
    label: i18n.t('questionEditor.questionType.ratingQuestion'),
    create: () => {
      const newRatingQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.RatingQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        scale: RatingScaleType.OneTo3,
        hasDisplayLogic: false,
        lowerLabel: '',
        upperLabel: '',
        code: ''
      };

      return newRatingQuestion;
    },
  },
  {
    type: QuestionType.SingleSelectQuestionType,
    icon: questionsIconMapping[QuestionType.SingleSelectQuestionType],
    label: i18n.t('questionEditor.questionType.singleSelectQuestion'),
    create: () => {
      const newSingleSelectQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.SingleSelectQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        hasDisplayLogic: false,
        code: '',
        options: [
          {
            optionId: crypto.randomUUID(),
            isFlagged: false,
            isFreeText: false,
            text: 'Option 1',
          },
          {
            optionId: crypto.randomUUID(),
            isFlagged: false,
            isFreeText: false,
            text: 'Option 2',
          },
        ]
      };

      return newSingleSelectQuestion;
    },
  },
  {
    type: QuestionType.MultiSelectQuestionType,
    icon: questionsIconMapping[QuestionType.MultiSelectQuestionType],
    label: i18n.t('questionEditor.questionType.multiSelectQuestion'),
    create: () => {
      const newMultiSelectQuestion: z.infer<typeof questionSchema> = {
        $questionType: QuestionType.MultiSelectQuestionType,
        questionId: crypto.randomUUID(),
        text: '',
        helptext: '',
        hasDisplayLogic: false,
        code: '',
        options: [
          {
            optionId: crypto.randomUUID(),
            isFlagged: false,
            isFreeText: false,
            text: 'Option 1',
          },
          {
            optionId: crypto.randomUUID(),
            isFlagged: false,
            isFreeText: false,
            text: 'Option 2',
          }
        ]
      };

      return newMultiSelectQuestion;
    },
  },
];

export default function AddQuestionButton({
  onAddQuestion,
}: {
  onAddQuestion: (question: z.infer<typeof questionSchema>) => void
}) {
  const [open, setOpen] = useState(false);

  return (
    <div className="mt-4">
      <Popover open={open} onOpenChange={setOpen}>
        <PopoverTrigger asChild>
          <Button variant="outline" type="button" className="w-full justify-start">
            <PlusIcon className="h-4 w-4" />
            Add Question
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-[300px] p-0" align="start">
          <Command>
            <CommandInput placeholder="Search question types..." />
            <CommandList>
              <CommandEmpty>No question types found.</CommandEmpty>
              <CommandGroup>
                {questionTypes.map((questionType) => (
                  <CommandItem
                    key={questionType.type}
                    onSelect={() => {
                      onAddQuestion(questionType.create());
                      setOpen(false);
                    }}>
                    <questionType.icon className="h-4 w-4" aria-hidden="true" />
                    {questionType.label}
                  </CommandItem>
                ))}
              </CommandGroup>
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>
    </div>
  );
}
