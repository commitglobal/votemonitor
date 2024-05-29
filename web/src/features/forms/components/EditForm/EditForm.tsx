import { authApi } from '@/common/auth-api';
import {
  QuestionType,
  cloneTranslation,
  getTranslationOrDefault,
  updateTranslationString,
  type BaseQuestion,
  type FunctionComponent,
  type MultiSelectQuestion,
  type NumberQuestion,
  type SingleSelectQuestion,
  type TextQuestion,
} from '@/common/types';
import FormQuestionsEditor from '@/components/questionsEditor/FormQuestionsEditor';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ErrorMessage, Field, FieldGroup, Fieldset, Label } from '@/components/ui/fieldset';
import { Form, FormControl, FormField, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/components/ui/use-toast';
import LanguageSelect from '@/containers/LanguageSelect';
import { queryClient } from '@/main';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRef, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

import Layout from '@/components/layout/Layout';
import { Route } from '@/routes/forms_.$formId.edit';
import { type FormFull, FormType, mapFormType } from '../../models/form';
import { formDetailsQueryOptions, formsKeys } from '../../queries';
import EditFormFooter from './EditFormFooter';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useNavigate } from '@tanstack/react-router';

function updateTranslations(
  question: BaseQuestion,
  previousLanguageCode: string,
  newLanguageCode: string
): BaseQuestion {
  question.text = cloneTranslation(question.text, previousLanguageCode, newLanguageCode)!;
  question.helptext = cloneTranslation(question.helptext, previousLanguageCode, newLanguageCode);

  if (question.$questionType === QuestionType.TextQuestionType) {
    const textQuestion: TextQuestion = question as TextQuestion;
    textQuestion.inputPlaceholder = cloneTranslation(
      textQuestion.inputPlaceholder,
      previousLanguageCode,
      newLanguageCode
    );

    return { ...textQuestion };
  }

  if (question.$questionType === QuestionType.NumberQuestionType) {
    const numberQuestion: NumberQuestion = question as NumberQuestion;
    numberQuestion.inputPlaceholder = cloneTranslation(
      numberQuestion.inputPlaceholder,
      previousLanguageCode,
      newLanguageCode
    );

    return { ...numberQuestion };
  }

  if (question.$questionType === QuestionType.SingleSelectQuestionType) {
    const singleSelectQuestion: SingleSelectQuestion = question as SingleSelectQuestion;
    singleSelectQuestion.options = singleSelectQuestion.options.map((option) => ({
      ...option,
      text: cloneTranslation(option.text, previousLanguageCode, newLanguageCode)!,
    }));

    return { ...singleSelectQuestion };
  }

  if (question.$questionType === QuestionType.MultiSelectQuestionType) {
    const MultiSelectQuestion: MultiSelectQuestion = question as MultiSelectQuestion;
    MultiSelectQuestion.options = MultiSelectQuestion.options.map((option) => ({
      ...option,
      text: cloneTranslation(option.text, previousLanguageCode, newLanguageCode)!,
    }));

    return { ...MultiSelectQuestion };
  }

  return { ...question };
}

export default function EditForm(): FunctionComponent {
  const { t } = useTranslation();
  const { formId } = Route.useParams();
  const formQuery = useSuspenseQuery(formDetailsQueryOptions(formId));
  const formData = formQuery.data;
  const navigate = useNavigate();

  const [localQuestions, setLocalQuestions] = useState(formData.questions);
  const [defaultLanguage, setDefaultLanguage] = useState(formData.defaultLanguage);
  const [languages, setLanguages] = useState(formData.languages);
  const formRef = useRef<HTMLFormElement>(null);

  const { toast } = useToast();

  const editFormFormSchema = z.object({
    code: z.string().nonempty(),
    name: z.string().nonempty(),
    description: z.string().optional(),
    defaultLanguage: z.string().nonempty(),
    formType: z
      .enum([FormType.Opening, FormType.Voting, FormType.ClosingAndCounting, FormType.Other])
      .catch(FormType.Opening),
  });

  const form = useForm<z.infer<typeof editFormFormSchema>>({
    resolver: zodResolver(editFormFormSchema),
    defaultValues: {
      code: formData.code,
      name: formData.name[formData.defaultLanguage],
      description: getTranslationOrDefault(formData.description, formData.defaultLanguage),
      formType: formData.formType,
      defaultLanguage: formData.defaultLanguage,
    },
  });

  const editMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: (form: FormFull) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${formData.id}`, {
        ...form,
        questions: localQuestions,
      });
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form updated successfully',
      });

      void queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },
  });

  function saveHandler(): void {
    const values = form.getValues();

    formData.code = values.code;
    formData.name[defaultLanguage] = values.name;
    formData.description = updateTranslationString(
      formData.description,
      formData.languages,
      formData.defaultLanguage,
      values.description ?? ''
    );
    formData.formType = values.formType;
    formData.defaultLanguage = defaultLanguage;
    formData.languages = languages;

    const updatedForm: FormFull = {
      ...formData,
    };

    editMutation.mutate(updatedForm);
  }

  function onSubmit(values: z.infer<typeof editFormFormSchema>): void {
    formData.code = values.code;
    formData.name[defaultLanguage] = values.name;
    formData.description = updateTranslationString(
      formData.description,
      formData.languages,
      formData.defaultLanguage,
      values.description ?? ''
    );
    formData.formType = values.formType;
    formData.defaultLanguage = defaultLanguage;
    formData.languages = languages;

    const updatedForm: FormFull = {
      ...formData,
    };

    editMutation.mutate(updatedForm, {
      onSuccess: () => {
        void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
      },
    });
  }

  const handleLanguageChange = (newLanguageCode: string): void => {
    const previousLanguageCode = defaultLanguage;
    setDefaultLanguage(newLanguageCode);
    setLanguages([...new Set([...languages, newLanguageCode])]);
    setLocalQuestions(
      localQuestions.map((question) => updateTranslations(question, previousLanguageCode, newLanguageCode))
    );
  };

  const submit = (): void => {
    if (formRef.current) {
      saveHandler();
    }
  };

  const submitAndExitEditor = (): void => {
    if (formRef.current) {
      formRef.current.dispatchEvent(new Event('submit', { cancelable: true, bubbles: true }));
    }
  };

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      title={`${formData.code} - ${formData.name[formData.defaultLanguage]}`}>
      <Form {...form}>
        {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
        <form className='flex flex-col flex-1' onSubmit={form.handleSubmit(onSubmit)} ref={formRef}>
          <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger value='form-details'>Form details</TabsTrigger>
              <TabsTrigger value='questions'>Questions</TabsTrigger>
            </TabsList>
            <TabsContent value='form-details'>
              <Card className='pt-0'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='text-xl'>Form details</CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='flex flex-col gap-6 items-baseline'>
                  <Fieldset className='grid grid-cols-2 gap-12'>
                    <FieldGroup className='!mt-0'>
                      <FormField
                        control={form.control}
                        name='code'
                        render={({ field, fieldState }) => (
                          <Field>
                            <Label>{t('form.field.code')}</Label>
                            <Input placeholder={t('form.placeholder.code')} {...field} {...fieldState} />
                            {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                          </Field>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name='formType'
                        render={({ field }) => (
                          <Field>
                            <Label>{t('form.field.formType')}</Label>
                            <Select onValueChange={field.onChange} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Select form type' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                <SelectItem value={FormType.Opening}>{mapFormType(FormType.Opening)}</SelectItem>
                                <SelectItem value={FormType.Voting}>{mapFormType(FormType.Voting)}</SelectItem>
                                <SelectItem value={FormType.ClosingAndCounting}>
                                  {mapFormType(FormType.ClosingAndCounting)}
                                </SelectItem>
                                <SelectItem value={FormType.Other}>{mapFormType(FormType.Other)}</SelectItem>
                              </SelectContent>
                            </Select>
                            <FormMessage />
                          </Field>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name='name'
                        render={({ field, fieldState }) => (
                          <Field>
                            <Label>{t('form.field.name')}</Label>
                            <Input placeholder={t('form.placeholder.name')} {...field} {...fieldState} />
                            {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                          </Field>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name='defaultLanguage'
                        render={({ field }) => (
                          <Field>
                            <Label>{t('form.field.defaultLanguage')}</Label>
                            <LanguageSelect
                              languageCode={field.value}
                              onSelect={(value) => {
                                handleLanguageChange(value);
                                field.onChange(value);
                              }}
                            />
                          </Field>
                        )}
                      />
                    </FieldGroup>
                    <FieldGroup className='!mt-0'>
                      <FormField
                        control={form.control}
                        name='description'
                        render={({ field }) => (
                          <Field>
                            <Label>{t('form.field.description')}</Label>
                            <Textarea rows={10} cols={100} {...field} placeholder={t('form.placeholder.description')} />
                          </Field>
                        )}
                      />
                    </FieldGroup>
                  </Fieldset>
                </CardContent>
              </Card>
            </TabsContent>
            <TabsContent className='flex flex-1 flex-col' value='questions'>
              <Card className='pt-0 h-[calc(100vh-380px)] overflow-hidden'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='text-xl'>Form questions</CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='-mx-6 flex items-start justify-left px-6 sm:mx-0 sm:px-8 h-[100%]'>
                  <FormQuestionsEditor
                    availableLanguages={languages}
                    languageCode={defaultLanguage}
                    localQuestions={localQuestions}
                    setLocalQuestions={setLocalQuestions}
                  />
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
          <EditFormFooter onSaveProgress={submit} onSaveAndExit={submitAndExitEditor} />
        </form>
      </Form>
    </Layout>
  );
}
