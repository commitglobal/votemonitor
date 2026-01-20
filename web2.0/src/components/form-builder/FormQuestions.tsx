import { formEditOpts, questionSchema } from "@/components/form-builder/shared";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  FieldDescription,
  FieldGroup,
  FieldLegend,
  FieldSet,
} from "@/components/ui/field";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { withForm } from "@/hooks/form";
import i18n from "@/i18n";
import { questionsIconMapping } from "@/lib/questions-icons";
import { QuestionType, RatingScaleType } from "@/types/form";
import { useStore } from "@tanstack/react-form";
import { PlusIcon } from "lucide-react";
import { useState } from "react";
import z from "zod";
import { TextQuestionCard } from "./TextQuestionCard";


export const FormQuestions = withForm({
  ...formEditOpts,
  render: ({ form }) => {
    const questions = useStore(form.store, (state) => state.values.questions)
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
                      <div key={question.questionId} className="flex flex-col gap-2">
                        <TextQuestionCard
                          form={form}
                          fields={{
                            questionId: `questions[${index}].questionId`,
                            $questionType: `questions[${index}].$questionType`,
                            code: `questions[${index}].code`,
                            text: `questions[${index}].text`,
                            helptext: `questions[${index}].helptext`,
                            inputPlaceholder: `questions[${index}].inputPlaceholder`,
                            hasDisplayLogic: `questions[${index}].hasDisplayLogic`,
                            condition: `questions[${index}].condition`,
                            parentQuestionId: `questions[${index}].parentQuestionId`,
                            value: `questions[${index}].value`,
                          }}
                          index={index}
                          numberOfQuestions={field.state.value.length}
                          onMoveUp={() => field.moveValue(index, index - 1)}
                          onMoveDown={() => field.moveValue(index, index + 1)}
                          onRemove={() => field.removeValue(index)}
                          questions={questions}
                        />

                        <AddQuestionButton
                          onAddQuestion={(question) => field.insertValue(index + 1, question)}
                        />
                      </div>
                    );
                  })}
                  {field.state.value.length === 0 && (
                    <AddQuestionButton
                      onAddQuestion={(question) => field.insertValue(0, question)}
                    />
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
    <div className="mb-2">
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
