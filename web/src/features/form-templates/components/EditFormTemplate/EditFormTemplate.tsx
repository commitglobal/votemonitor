import { authApi } from '@/common/auth-api';
import {
  BaseQuestion,
  cloneTranslation,
  getTranslationOrDefault,
  MultiSelectQuestion,
  NumberQuestion,
  QuestionType,
  SingleSelectQuestion,
  TextQuestion,
  updateTranslationString,
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
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

import Layout from '@/components/layout/Layout';
import { Route } from '@/routes/form-templates_.$formTemplateId.edit';
import { FormTemplateFull, FormTemplateType, mapFormTemplateType } from '../../models/formTemplate';
import { formTemplateDetailsQueryOptions, formTemplatesKeys } from '../../queries';
import EditFormTemplateFooter from './EditFormTemplateFooter';

export default function EditFormTemplate() {
  const { t } = useTranslation();

  const { formTemplateId } = Route.useParams();
  const formTemplateQuery = useSuspenseQuery(formTemplateDetailsQueryOptions(formTemplateId));
  const formTemplate = formTemplateQuery.data;

  const [localQuestions, setLocalQuestions] = useState(formTemplate.questions);
  const [defaultLanguage, setDefaultLanguage] = useState(formTemplate.defaultLanguage);
  const [languages, setLanguages] = useState(formTemplate.languages);

  const { toast } = useToast();

  const editFormTemplateFormSchema = z.object({
    code: z.string().nonempty(),
    name: z.string().nonempty(),
    description: z.string().optional(),
    defaultLanguage: z.string().nonempty(),
    formTemplateType: z
      .enum([FormTemplateType.Opening, FormTemplateType.Voting, FormTemplateType.ClosingAndCounting, FormTemplateType.Other])
      .catch(FormTemplateType.Opening),
  });

  const form = useForm<z.infer<typeof editFormTemplateFormSchema>>({
    resolver: zodResolver(editFormTemplateFormSchema),
    defaultValues: {
      code: formTemplate.code,
      name: formTemplate.name[formTemplate.defaultLanguage],
      description: getTranslationOrDefault(formTemplate.description, formTemplate.defaultLanguage),
      formTemplateType: formTemplate.formTemplateType,
      defaultLanguage: formTemplate.defaultLanguage,
    },
  });

  function onSubmit(values: z.infer<typeof editFormTemplateFormSchema>) {
    formTemplate.code = values.code;
    formTemplate.name[defaultLanguage] = values.name;
    formTemplate.description = updateTranslationString(
      formTemplate.description,
      formTemplate.languages,
      formTemplate.defaultLanguage,
      values.description ?? ''
    );
    formTemplate.formTemplateType = values.formTemplateType;
    formTemplate.defaultLanguage = defaultLanguage;
    formTemplate.languages = languages;

    const updatedFormTemplate: FormTemplateFull = {
      ...formTemplate,
    };

    editMutation.mutate(updatedFormTemplate);
  }

  const editMutation = useMutation({
    mutationFn: (obj: FormTemplateFull) => {
      return authApi.put<void>(`/form-templates/${formTemplate.id}`, {
        ...obj,
        questions: localQuestions,
      });
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form template updated successfully updated',
      });

      queryClient.invalidateQueries({ queryKey: formTemplatesKeys.all });
    },
  });

  const updateTranslations = (
    question: BaseQuestion,
    previousLanguageCode: string,
    newLanguageCode: string
  ): BaseQuestion => {
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
  };

  const handleLanguageChange = (newLanguageCode: string): void => {
    const previousLanguageCode = defaultLanguage;
    setDefaultLanguage(newLanguageCode);
    setLanguages(Array.from(new Set([...languages, newLanguageCode])));
    setLocalQuestions([
      ...localQuestions.map((question) => updateTranslations(question, previousLanguageCode, newLanguageCode)),
    ]);
  };

  return (
    <Layout title={formTemplate.code}>
      <Form {...form}>
        <form className='flex flex-col flex-1' onSubmit={form.handleSubmit(onSubmit)}>
          <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger value='form-details'>Template form details</TabsTrigger>
              <TabsTrigger value='questions'>Questions</TabsTrigger>
            </TabsList>
            <TabsContent value='form-details'>
              <Card className='pt-0 h-[calc(100vh+360px)]'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='text-xl'>Template form details</CardTitle>
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
                            <Label>{t('form-template.field.code')}</Label>
                            <Input placeholder={t('form-template.placeholder.code')} {...field} {...fieldState} />
                            {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                          </Field>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name='formTemplateType'
                        render={({ field }) => (
                          <Field>
                            <Label>{t('form-template.field.formTemplateType')}</Label>
                            <Select onValueChange={field.onChange} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Select a form template type' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                <SelectItem value={FormTemplateType.Opening}>
                                  {mapFormTemplateType(FormTemplateType.Opening)}
                                </SelectItem>
                                <SelectItem value={FormTemplateType.Voting}>
                                  {mapFormTemplateType(FormTemplateType.Voting)}
                                </SelectItem>
                                <SelectItem value={FormTemplateType.ClosingAndCounting}>
                                  {mapFormTemplateType(FormTemplateType.ClosingAndCounting)}
                                </SelectItem>
                                <SelectItem value={FormTemplateType.Other}>
                                  {mapFormTemplateType(FormTemplateType.Other)}
                                </SelectItem>
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
                            <Label>{t('form-template.field.name')}</Label>
                            <Input placeholder={t('form-template.placeholder.name')} {...field} {...fieldState} />
                            {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                          </Field>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name='defaultLanguage'
                        render={({ field }) => (
                          <Field>
                            <Label>{t('form-template.field.defaultLanguage')}</Label>
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
                            <Label>{t('form-template.field.description')}</Label>
                            <Textarea
                              rows={10}
                              cols={100}
                              {...field}
                              placeholder={t('form-template.placeholder.description')}
                            />
                          </Field>
                        )}
                      />
                    </FieldGroup>
                  </Fieldset>
                </CardContent>
              </Card>
            </TabsContent>
            <TabsContent className='flex flex-1 flex-col' value='questions'>
              <Card className='pt-0 flex flex-col flex-1'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='text-xl'>Template form questions</CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='flex flex-1'>
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
          <EditFormTemplateFooter />
        </form>
      </Form>
    </Layout>
  );
}
