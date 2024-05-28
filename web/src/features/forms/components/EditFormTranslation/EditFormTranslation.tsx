import { authApi } from '@/common/auth-api';
import { getTranslationOrDefault, updateTranslationString } from '@/common/types';
import Layout from '@/components/layout/Layout';
import FormQuestionsEditor from '@/components/questionsEditor/FormQuestionsEditor';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ErrorMessage, Field, FieldGroup, Fieldset, Label } from '@/components/ui/fieldset';
import { Form, FormField } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { Route as EditFormRoute } from '@/routes/forms_.$formId.edit-translation.$languageCode';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRef, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

import { FormFull } from '../../models/form';
import { formDetailsQueryOptions, formsKeys } from '../../queries';
import EditFormFooter from '../EditForm/EditFormFooter';
import LanguageBadge from '../LanguageBadge/LanguageBadge';

export default function EditFormTranslation() {
  const { t } = useTranslation();
  const { languageCode, formId } = EditFormRoute.useParams();
  const formQuery = useSuspenseQuery(formDetailsQueryOptions(formId));
  const formData = formQuery.data;
  const [localQuestions, setLocalQuestions] = useState(formData.questions);
  const { toast } = useToast();
  const formRef = useRef(null);

  const editFormFormSchema = z.object({
    name: z.string().nonempty(),
    description: z.string().optional(),
  });

  const form = useForm<z.infer<typeof editFormFormSchema>>({
    resolver: zodResolver(editFormFormSchema),
    defaultValues: {
      name: formData.name[formData.defaultLanguage],
      description: getTranslationOrDefault(formData.description, formData.defaultLanguage),
    },
  });

  function onSubmit(values: z.infer<typeof editFormFormSchema>) {
    formData.name[formData.defaultLanguage] = values.name;
    formData.description = updateTranslationString(
      formData.description,
      formData.languages,
      formData.defaultLanguage,
      values.description ?? ''
    );

    const updatedForm: FormFull = {
      ...formData,
    };

    editMutation.mutate(updatedForm);
  }

  const editMutation = useMutation({
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
        description: 'Form updated successfully updated',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },
  });

  const submit = () => {
    if (formRef.current) {
      // @ts-ignore
      formRef.current.dispatchEvent(new Event('submit', { cancelable: true, bubbles: true }));
    }
  };

  return (
    <Layout title={`${formData.code} - ${formData.name[formData.defaultLanguage]}`}>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} ref={formRef}>
          <Tabs defaultValue='form-details'>
            <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
              <TabsTrigger value='form-details'>Form details</TabsTrigger>
              <TabsTrigger value='questions'>Questions</TabsTrigger>
            </TabsList>
            <TabsContent value='form-details'>
              <Card className='pt-0'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='flex  gap-1'>
                      <span className='text-xl'>Form details</span>
                      <LanguageBadge languageCode={languageCode} />
                    </CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='flex flex-col gap-6 items-baseline'>
                  <Fieldset className='grid grid-cols-2 gap-12'>
                    <FieldGroup className='!mt-0'>
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
            <TabsContent value='questions'>
              <Card className='pt-0 h-[calc(100vh-380px)] overflow-hidden'>
                <CardHeader className='flex flex-column gap-2'>
                  <div className='flex flex-row justify-between items-center'>
                    <CardTitle className='text-xl'>Form questions</CardTitle>
                  </div>
                  <Separator />
                </CardHeader>
                <CardContent className='-mx-6 flex items-start justify-left px-6 sm:mx-0 sm:px-8 h-[100%]'>
                  <FormQuestionsEditor
                    availableLanguages={formData.languages}
                    languageCode={formData.defaultLanguage}
                    localQuestions={localQuestions}
                    setLocalQuestions={setLocalQuestions}
                  />
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
          <EditFormFooter onSaveProgress={submit} onSaveAndExit={submit} />
        </form>
      </Form>
    </Layout>
  );
}
