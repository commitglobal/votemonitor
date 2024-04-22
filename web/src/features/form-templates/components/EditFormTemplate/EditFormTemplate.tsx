import { authApi } from '@/common/auth-api';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ErrorMessage, Field, FieldGroup, Fieldset, Label } from '@/components/ui/fieldset';
import { Form, FormControl, FormField, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/components/ui/use-toast';
import LanguageSelect from '@/containers/LanguageSelect';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useLoaderData, useNavigate } from '@tanstack/react-router';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';
import { FormTemplateFull, FormTemplateType, mapFormTemplateType } from '../../models/formTemplate';

import FormQuestionsEditor from '@/components/questionsEditor/FormQuestionsEditor';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { queryClient } from '@/main';
import { useState } from 'react';
import { formTemplatesKeys } from '../../queries';
import EditFormTemplateFooter from './EditFormTemplateFooter';

export default function EditFormTemplate() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const formTemplate: FormTemplateFull = useLoaderData({ strict: false });
  const [localQuestions, setLocalQuestions] = useState(formTemplate.questions);
  const { toast } = useToast();

  const editFormTemplateFormSchema = z.object({
    code: z.string().nonempty(),
    name: z.string().nonempty(),
    description: z.string().optional(),
    defaultLanguage: z.string().nonempty(),
    formTemplateType: z
      .enum([FormTemplateType.Opening, FormTemplateType.Voting, FormTemplateType.ClosingAndCounting])
      .catch(FormTemplateType.Opening),
  });

  const form = useForm<z.infer<typeof editFormTemplateFormSchema>>({
    resolver: zodResolver(editFormTemplateFormSchema),
    defaultValues: {
      code: formTemplate.code,
      name: formTemplate.name[formTemplate.defaultLanguage],
      description: formTemplate.description[formTemplate.defaultLanguage],
      formTemplateType: formTemplate.formTemplateType,
      defaultLanguage: formTemplate.defaultLanguage,
    },
  });

  function onSubmit(values: z.infer<typeof editFormTemplateFormSchema>) {
    formTemplate.code = values.code;
    formTemplate.name[formTemplate.defaultLanguage] = values.name;
    formTemplate.description[formTemplate.defaultLanguage] = values.description ?? '';
    formTemplate.formTemplateType = values.formTemplateType;
    formTemplate.defaultLanguage = values.defaultLanguage;

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

  return (
    <Layout title={`${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage]}`}>
      <Form {...form}>
        <form className='flex flex-col flex-1' onSubmit={form.handleSubmit(onSubmit)}>
          <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger value='form-details'>Template form details</TabsTrigger>
              <TabsTrigger value='questions'>Questions</TabsTrigger>
            </TabsList>
            <TabsContent value='form-details'>
              <Card className='pt-0'>
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
                            <LanguageSelect languageCode={field.value} onSelect={field.onChange} />
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
                    availableLanguages={formTemplate.languages}
                    languageCode={formTemplate.defaultLanguage}
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
