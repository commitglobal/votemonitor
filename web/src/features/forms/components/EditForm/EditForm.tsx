import { QuestionType, ZFormType, type FunctionComponent } from '@/common/types';
import FormQuestionsEditor from '@/components/questionsEditor/FormQuestionsEditor';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { zodResolver } from '@hookform/resolvers/zod';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';

import { authApi } from '@/common/auth-api';
import {
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from '@/common/guards';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button, buttonVariants } from '@/components/ui/button';
import { LanguageBadge } from '@/components/ui/language-badge';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import {
  cn,
  ensureTranslatedStringCorrectness,
  isNilOrWhitespace,
  isNotNilOrWhitespace
} from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/forms_.$formId.edit';
import { useMutation } from '@tanstack/react-query';
import { useBlocker, useNavigate } from '@tanstack/react-router';
import { useEffect, useState } from 'react';
import { UpdateFormRequest } from '../../models/form';
import { formDetailsQueryOptions, formsKeys } from '../../queries';
import {
  EditDateQuestionType,
  EditMultiSelectQuestionType,
  EditNumberQuestionType,
  EditRatingQuestionType,
  EditSingleSelectQuestionType,
  EditTextQuestionType,
  mapToQuestionRequest,
  ZEditQuestionType,
  ZTranslatedString,
} from '../../types';
import { FormDetailsBreadcrumbs } from '../FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import EditFormDetails from './EditFormDetails';

export const ZEditFormType = z
  .object({
    formId: z.string().trim().min(1),
    languageCode: z.string().trim().min(1),
    defaultLanguage: z.string().trim().min(1),
    code: z.string().trim().min(1),
    name: ZTranslatedString,
    description: ZTranslatedString.optional(),
    languages: z.array(z.string()),
    formType: ZFormType.catch(ZFormType.Values.Opening),
    questions: z.array(ZEditQuestionType),
  })
  .superRefine((data, ctx) => {
    if (isNilOrWhitespace(data.name[data.languageCode])) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Form name is required',
        path: ['name'],
      });
    }

    if (
      isNotNilOrWhitespace(data.description?.[data.defaultLanguage]) &&
      isNilOrWhitespace(data.description?.[data.languageCode])
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Form description is required',
        path: ['description'],
      });
    }

    data.questions.forEach((question, index) => {
      if (isNilOrWhitespace(question.text[question.languageCode])) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'Question text is required',
          path: ['questions', index, 'text'],
        });
      }

      if (
        isNotNilOrWhitespace(question.helptext[question.defaultLanguage]) &&
        isNilOrWhitespace(question.helptext[question.languageCode])
      ) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'Question helptext is required',
          path: ['questions', index, 'helptext'],
        });
      }

      if (
        question.$questionType === QuestionType.NumberQuestionType ||
        question.$questionType === QuestionType.TextQuestionType
      ) {
        if (
          isNotNilOrWhitespace(question.inputPlaceholder[question.defaultLanguage]) &&
          isNilOrWhitespace(question.inputPlaceholder[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Input placeholder is required',
            path: ['questions', index, 'inputPlaceholder'],
          });
        }
      }

      if (question.$questionType === QuestionType.RatingQuestionType) {
        if (
          isNotNilOrWhitespace(question.lowerLabel[question.defaultLanguage]) &&
          isNilOrWhitespace(question.lowerLabel[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Question lower label is required',
            path: ['questions', index, 'lowerLabel'],
          });
        }

        if (
          isNotNilOrWhitespace(question.upperLabel[question.defaultLanguage]) &&
          isNilOrWhitespace(question.upperLabel[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Question upper label is required',
            path: ['questions', index, 'upperLabel'],
          });
        }
      }

      if (
        question.$questionType === QuestionType.SingleSelectQuestionType ||
        question.$questionType === QuestionType.MultiSelectQuestionType
      ) {
        question.options.forEach((option, optionIndex) => {
          if (isNilOrWhitespace(option.text[question.languageCode])) {
            ctx.addIssue({
              code: z.ZodIssueCode.custom,
              message: 'Option text is required',
              path: ['questions', index, 'options', optionIndex, 'text'],
            });
          }
        });

        // check uniqueness of options
        const optionTexts = question.options.map((o) => o.text[question.languageCode]);
        const textCountMap = new Map<string | undefined, number>();
        const duplicatedIndexesMap = new Map<string | undefined, number>();

        // Step 1: Count occurrences of each option
        optionTexts.forEach((text, optionIndex) => {
          const numberOfOccurrences = (textCountMap.get(text) || 0) + 1;
          if (numberOfOccurrences > 1) {
            duplicatedIndexesMap.set(text, optionIndex);
          }
          textCountMap.set(text, numberOfOccurrences);
        });

        // Step 2: Mark duplicated options as invalid
        for (const [_, optionIndex] of duplicatedIndexesMap.entries()) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Option text is not unique',
            path: ['questions', index, 'options', optionIndex, 'text'],
          });
        }
      }

      if (question.hasDisplayLogic) {
        if (question.condition === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Question condition is required',
            path: ['questions', index, 'condition'],
          });
        }

        if (question.parentQuestionId === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Question parent question is required',
            path: ['questions', index, 'parentQuestionId'],
          });
        }

        if (question.value === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Question value is required',
            path: ['questions', index, 'value'],
          });
        }
      }

      return z.NEVER;
    });
  });

export type EditFormType = z.infer<typeof ZEditFormType>;

export default function EditForm(): FunctionComponent {
  const { formId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: formData } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));
  const confirm = useConfirm();
  const [shouldExitEditor, setShouldExitEditor] = useState(false);
  const navigate = useNavigate();

  const editQuestions = formData.questions.map((question) => {
    if (isNumberQuestion(question)) {
      const numberQuestion: EditNumberQuestionType = {
        $questionType: QuestionType.NumberQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),
        inputPlaceholder: ensureTranslatedStringCorrectness(question.inputPlaceholder, formData.languages),

        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
      };

      return numberQuestion;
    }
    if (isTextQuestion(question)) {
      const textQuestion: EditTextQuestionType = {
        $questionType: QuestionType.TextQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),
        inputPlaceholder: ensureTranslatedStringCorrectness(question.inputPlaceholder, formData.languages),

        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
      };

      return textQuestion;
    }

    if (isDateQuestion(question)) {
      const dateQuestion: EditDateQuestionType = {
        $questionType: QuestionType.DateQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),

        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
      };

      return dateQuestion;
    }

    if (isRatingQuestion(question)) {
      const ratingQuestion: EditRatingQuestionType = {
        $questionType: QuestionType.RatingQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),
        scale: question.scale,
        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
        lowerLabel: ensureTranslatedStringCorrectness(question.lowerLabel, formData.languages),
        upperLabel: ensureTranslatedStringCorrectness(question.upperLabel, formData.languages),
      };

      return ratingQuestion;
    }

    if (isSingleSelectQuestion(question)) {
      const singleSelectQuestion: EditSingleSelectQuestionType = {
        $questionType: QuestionType.SingleSelectQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),
        options:
          question.options?.map((o) => ({
            optionId: o.id,
            isFlagged: o.isFlagged,
            isFreeText: o.isFreeText,
            text: ensureTranslatedStringCorrectness(o.text, formData.languages),
          })) ?? [],

        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
      };

      return singleSelectQuestion;
    }

    if (isMultiSelectQuestion(question)) {
      const multiSelectQuestion: EditMultiSelectQuestionType = {
        $questionType: QuestionType.MultiSelectQuestionType,
        questionId: question.id,
        text: ensureTranslatedStringCorrectness(question.text, formData.languages),
        helptext: ensureTranslatedStringCorrectness(question.helptext, formData.languages),
        options:
          question.options?.map((o) => ({
            optionId: o.id,
            isFlagged: o.isFlagged,
            isFreeText: o.isFreeText,
            text: ensureTranslatedStringCorrectness(o.text, formData.languages),
          })) ?? [],

        hasDisplayLogic: !!question.displayLogic,

        parentQuestionId: question.displayLogic?.parentQuestionId,
        condition: question.displayLogic?.condition,
        value: question.displayLogic?.value,

        code: question.code,
        languageCode: formData.defaultLanguage,
        defaultLanguage: formData.defaultLanguage,
      };

      return multiSelectQuestion;
    }

    return undefined;
  });

  const form = useForm<EditFormType>({
    resolver: zodResolver(ZEditFormType),
    defaultValues: {
      formId: formData.id,
      code: formData.code,
      languageCode: formData.defaultLanguage,
      defaultLanguage: formData.defaultLanguage,
      languages: formData.languages,
      name: ensureTranslatedStringCorrectness(formData.name, formData.languages),
      description: ensureTranslatedStringCorrectness(formData.description, formData.languages),
      formType: formData.formType,
      questions: editQuestions,
    },
    mode: 'all',
  });

  useBlocker(
    () =>
      confirm({
        title: `Unsaved Changes Detected`,
        body: 'You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?',
        actionButton: 'Leave',
        actionButtonClass: buttonVariants({ variant: 'destructive' }),
        cancelButton: 'Stay',
      }),
    form.formState.isDirty
  );

  const code = useWatch({ control: form.control, name: 'code', defaultValue: formData.code });
  const name = useWatch({ control: form.control, name: 'name', defaultValue: formData.name });

  const languageCode = useWatch({
    control: form.control,
    name: 'languageCode',
    defaultValue: formData.defaultLanguage,
  });

  const editMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({
      electionRoundId,
      form,
    }: {
      electionRoundId: string;
      form: UpdateFormRequest;
      shouldExitEditor: boolean;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${form.id}`, {
        ...form,
      });
    },

    onSuccess: async (_, { shouldExitEditor }) => {
      toast({
        title: 'Success',
        description: 'Form updated successfully',
      });

      await queryClient.invalidateQueries({ queryKey: formsKeys.all, type: 'all' });

      if (shouldExitEditor) {
        if (
          await confirm({
            title: 'Changes made to form in base language',
            body: 'Please note that changes have been made to the form in base language, which can impact the translation(s). All new questions or response options which you have added have been copied to translations but in the base language. Access each translation of the form and manually translate each of the changes.',
          })
        ) {
          void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
        }
      }
    },

    onError: () => {
      toast({
        title: 'Error saving the form',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function saveForm(values: EditFormType) {
    const updatedForm: UpdateFormRequest = {
      id: values.formId,
      code: values.code,
      name: values.name,
      defaultLanguage: values.languageCode,
      description: values.description,
      formType: values.formType,
      languages: values.languages,
      questions: values.questions.map(mapToQuestionRequest),
    };
    editMutation.mutate({ form: updatedForm, shouldExitEditor, electionRoundId: currentElectionRoundId });
  }

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={code} formName={name[languageCode] ?? ''} />}
      title={`${code} - ${name[languageCode]}`}>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(saveForm)} className='flex flex-col flex-1'>
          <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger
                value='form-details'
                className={cn({
                  'border-b-4 border-red-400': form.getFieldState('name').invalid || form.getFieldState('code').invalid,
                })}>
                Form details
              </TabsTrigger>
              <TabsTrigger
                value='questions'
                className={cn({ 'border-b-4 border-red-400': form.getFieldState('questions').invalid })}>
                Questions
              </TabsTrigger>
            </TabsList>
            <TabsContent value='form-details'>
              <Card className='pt-0'>
                <CardHeader className='flex gap-2 flex-column'>
                  <div className='flex flex-row items-center justify-between'>
                    <CardTitle className='text-xl'>Form details</CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='flex flex-col items-baseline gap-6'>
                  <EditFormDetails languageCode={languageCode} />
                </CardContent>
              </Card>
            </TabsContent>
            <TabsContent className='flex flex-col flex-1' value='questions'>
              <Card className='pt-0 h-[calc(100vh)] overflow-hidden'>
                <CardHeader className='flex gap-2 flex-column'>
                  <div className='flex flex-row items-center justify-between'>
                    <CardTitle className='text-xl'>
                      {languageCode && (
                        <div className='flex items-center gap-2'>
                          <span className='text-sm'>Form questions</span>
                          <LanguageBadge languageCode={languageCode} />
                        </div>
                      )}
                    </CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='-mx-6 flex items-start justify-left px-6 sm:mx-0 sm:px-8 h-[100%]'>
                  <FormQuestionsEditor />
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
          <footer className='fixed left-0 bottom-0 h-[64px] w-full bg-white'>
            <div className='container flex items-center justify-end h-full gap-4'>
              <Button
                type='submit'
                variant='outline'
                onClick={() => {
                  setShouldExitEditor(false);
                }}
                disabled={!form.formState.isValid}>
                Save
              </Button>
              <Button
                type='submit'
                variant='default'
                onClick={() => {
                  setShouldExitEditor(true);
                }}
                disabled={!form.formState.isValid}>
                Save and exit form editor
              </Button>
            </div>
          </footer>
        </form>
      </Form>
    </Layout>
  );
}
