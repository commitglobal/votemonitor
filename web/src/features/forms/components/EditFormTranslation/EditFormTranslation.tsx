import {
  QuestionType,
  type FunctionComponent
} from '@/common/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { zodResolver } from '@hookform/resolvers/zod';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useForm, useWatch } from 'react-hook-form';

import { isDateQuestion, isMultiSelectQuestion, isNumberQuestion, isRatingQuestion, isSingleSelectQuestion, isTextQuestion } from '@/common/guards';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import FormQuestionsTranslator from '@/components/questionsEditor/FormQuestionsTranslator';
import { LanguageBadge } from '@/components/ui/language-badge';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn, emptyTranslatedString } from '@/lib/utils';
import { Route } from '@/routes/forms_.$formId.edit-translation.$languageCode';
import { useEffect } from 'react';
import { formDetailsQueryOptions } from '../../queries';
import { EditDateQuestionType, EditMultiSelectQuestionType, EditNumberQuestionType, EditRatingQuestionType, EditSingleSelectQuestionType, EditTextQuestionType } from '../../types';
import { EditFormType, ZEditFormType } from '../EditForm/EditForm';
import { FormDetailsBreadcrumbs } from '../FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import EditFormTranslationDetails from './EditFormTranslationDetails';
import EditFormTranslationFooter from './EditFormTranslationFooter';

export default function EditFormTranslation(): FunctionComponent {
  const { formId, languageCode } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: formData } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));

  const formQuestions = formData
    .questions
    .map(question => {
      if (isNumberQuestion(question)) {
        const numberQuestion: EditNumberQuestionType = {
          $questionType: QuestionType.NumberQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? emptyTranslatedString(formData.languages),
          inputPlaceholder: question.inputPlaceholder ?? emptyTranslatedString(formData.languages),

          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
        };

        return numberQuestion;
      }

      if (isTextQuestion(question)) {
        const textQuestion: EditTextQuestionType = {
          $questionType: QuestionType.TextQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? emptyTranslatedString(formData.languages),
          inputPlaceholder: question.inputPlaceholder ?? emptyTranslatedString(formData.languages),

          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
        };

        return textQuestion;
      }

      if (isDateQuestion(question)) {
        const dateQuestion: EditDateQuestionType = {
          $questionType: QuestionType.DateQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? emptyTranslatedString(formData.languages),

          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
        };

        return dateQuestion;
      }

      if (isRatingQuestion(question)) {
        const ratingQuestion: EditRatingQuestionType = {
          $questionType: QuestionType.RatingQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? emptyTranslatedString(formData.languages),
          scale: question.scale,
          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          lowerLabel: question.lowerLabel ?? emptyTranslatedString(formData.languages),
          upperLabel: question.upperLabel ?? emptyTranslatedString(formData.languages),
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
        };

        return ratingQuestion;
      }

      if (isSingleSelectQuestion(question)) {
        const singleSelectQuestion: EditSingleSelectQuestionType = {
          $questionType: QuestionType.SingleSelectQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? emptyTranslatedString(formData.languages),
          options: question.options?.map(o => ({ optionId: o.id, isFlagged: o.isFlagged, isFreeText: o.isFreeText, text: o.text })) ?? [],

          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
        };

        return singleSelectQuestion;
      }

      if (isMultiSelectQuestion(question)) {
        const multiSelectQuestion: EditMultiSelectQuestionType = {
          $questionType: QuestionType.MultiSelectQuestionType,
          questionId: question.id,
          text: question.text,
          helptext: question.helptext ?? {},
          options: question.options?.map(o => ({ optionId: o.id, isFlagged: o.isFlagged, isFreeText: o.isFreeText, text: o.text })) ?? [],

          hasDisplayLogic: !!question.displayLogic,

          parentQuestionId: question.displayLogic?.parentQuestionId,
          condition: question.displayLogic?.condition,
          value: question.displayLogic?.value,

          code: question.code,
          defaultLanguage: formData.defaultLanguage,
          languageCode: languageCode
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
      name: formData.name,
      description: formData.description,
      formType: formData.formType,
      questions: formQuestions
    },
    mode: 'all'
  });


  useEffect(() => {
    form.trigger();
  }, [formData]);

  const name = useWatch({ control: form.control, name: 'name', defaultValue: formData.name });

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={formData.code} formName={name[languageCode] ?? ''} />}
      title={`${formData.code} - ${name[languageCode] ?? ''}`}>
      <Form {...form}>
        <form className='flex flex-col flex-1'>
          <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger value='form-details' className={cn({ 'border-b-4 border-red-400': form.getFieldState('name').invalid || form.getFieldState('code').invalid })}>Form details</TabsTrigger>
              <TabsTrigger value='questions' className={cn({ 'border-b-4 border-red-400': form.getFieldState('questions').invalid })}>Questions</TabsTrigger>
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
          <EditFormTranslationFooter />
        </form>
      </Form>
    </Layout>
  );
}
