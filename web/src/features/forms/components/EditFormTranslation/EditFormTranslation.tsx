import { QuestionType, type FunctionComponent } from '@/common/types';
import { Button, buttonVariants } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useForm, useWatch } from 'react-hook-form';

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
import FormQuestionsTranslator from '@/components/questionsEditor/FormQuestionsTranslator';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { LanguageBadge } from '@/components/ui/language-badge';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn, ensureTranslatedStringCorrectness } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/forms_.$formId.edit-translation.$languageCode';
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
} from '../../types';
import { EditFormType, ZEditFormType } from '../EditForm/EditForm';
import { FormDetailsBreadcrumbs } from '../FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import EditFormTranslationDetails from './EditFormTranslationDetails';

export default function EditFormTranslation(): FunctionComponent {
  const { formId, languageCode } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: formData } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));
  const confirm = useConfirm();
  const [shouldExitEditor, setShouldExitEditor] = useState(false);
  const navigate = useNavigate();

  const formQuestions = formData.questions.map((question) => {
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
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
        lowerLabel: ensureTranslatedStringCorrectness(question.lowerLabel, formData.languages),
        upperLabel: ensureTranslatedStringCorrectness(question.upperLabel, formData.languages),
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
        defaultLanguage: formData.defaultLanguage,
        languageCode: languageCode,
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
      languageCode,
      defaultLanguage: formData.defaultLanguage,
      languages: formData.languages,
      name: ensureTranslatedStringCorrectness(formData.name, formData.languages),
      description: ensureTranslatedStringCorrectness(formData.description, formData.languages),
      formType: formData.formType,
      questions: formQuestions,
    },
    mode: 'all',
  });

  const editMutation = useMutation({
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

    onSuccess: (_, { shouldExitEditor ,electionRoundId}) => {
      toast({
        title: 'Success',
        description: 'Form updated successfully',
      });

      void queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });

      if (shouldExitEditor) {
        void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
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
      name: ensureTranslatedStringCorrectness(values.name, values.languages),
      description: ensureTranslatedStringCorrectness(values.description, values.languages),
      defaultLanguage: values.defaultLanguage,
      formType: values.formType,
      languages: values.languages,
      questions: values.questions.map(mapToQuestionRequest),
    };

    editMutation.mutate({ electionRoundId: currentElectionRoundId, form: updatedForm, shouldExitEditor });
  }

  useEffect(() => {
    form.trigger();
  }, [formData]);

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

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

  const name = useWatch({ control: form.control, name: 'name', defaultValue: formData.name });

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={formData.code} formName={name[languageCode] ?? ''} />}
      title={`${formData.code} - ${name[languageCode] ?? ''}`}>
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
                  <EditFormTranslationDetails languageCode={languageCode} />
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
                  <FormQuestionsTranslator />
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
