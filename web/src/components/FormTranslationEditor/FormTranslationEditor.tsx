import { QuestionType, type FunctionComponent } from '@/common/types';
import { Button, buttonVariants } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm, useWatch } from 'react-hook-form';

import {
  EditDateQuestionType,
  EditMultiSelectQuestionType,
  EditNumberQuestionType,
  EditRatingQuestionType,
  EditSingleSelectQuestionType,
  EditTextQuestionType,
} from '@/common/form-requests';
import {
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from '@/common/guards';
import FormQuestionsTranslator from '@/components/questionsEditor/FormQuestionsTranslator';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { LanguageBadge } from '@/components/ui/language-badge';
import { FormTemplateFull } from '@/features/form-templates/models';
import { FormFull } from '@/features/forms/models';
import { cn, ensureTranslatedStringCorrectness } from '@/lib/utils';
import { useBlocker } from '@tanstack/react-router';
import { useEffect } from 'react';
import { EditFormType, ZEditFormType } from '../FormEditor/FormEditor';
import FormDetailsTranslationEditor from './FormDetailsTranslationEditor';

export interface FormTranslationEditorProps {
  formData: FormFull | FormTemplateFull;
  onSaveForm: (formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => void;
  hasCitizenReportingOption: boolean;
  languageCode: string;
}

export default function FormTranslationEditor({
  languageCode,
  hasCitizenReportingOption,
  formData,
  onSaveForm,
}: FormTranslationEditorProps): FunctionComponent {
  const confirm = useConfirm();

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
    <Form {...form}>
      <form className='flex flex-col flex-1'>
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
                <FormDetailsTranslationEditor
                  languageCode={languageCode}
                  hasCitizenReportingOption={hasCitizenReportingOption}
                />
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
                form.handleSubmit((data) => onSaveForm(data, false));
              }}
              disabled={!form.formState.isValid}>
              Save
            </Button>
            <Button
              type='submit'
              variant='default'
              onClick={() => {
                form.handleSubmit((data) => onSaveForm(data, true));
              }}
              disabled={!form.formState.isValid}>
              Save and exit form editor
            </Button>
          </div>
        </footer>
      </form>
    </Form>
  );
}
