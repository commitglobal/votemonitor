import { authApi } from '@/common/auth-api';
import { type FunctionComponent, getTranslationOrDefault, updateTranslationString } from '@/common/types';
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

import type { FormFull } from '../../models/form';
import { formDetailsQueryOptions, formsKeys } from '../../queries';
import EditFormFooter from '../EditForm/EditFormFooter';
import LanguageBadge from '../LanguageBadge/LanguageBadge';
import { useNavigate } from '@tanstack/react-router';
import { isQuestionTranslated } from '@/components/questionsEditor/utils';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { useDialog } from '@/components/ui/use-dialog';

export default function EditFormTranslation(): FunctionComponent {
  const { t } = useTranslation();
  const { languageCode, formId } = EditFormRoute.useParams();
  const formQuery = useSuspenseQuery(formDetailsQueryOptions(formId));
  const formData = formQuery.data;
  const [localQuestions, setLocalQuestions] = useState(formData.questions);
  const { toast } = useToast();
  const formRef = useRef<HTMLFormElement>(null);
  const navigate = useNavigate();
  const {
    dialogProps: { onOpenChange, open },
  } = useDialog();

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
        description: 'Form updated successfully',
      });

      void queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },
  });

  function saveHandler(): void {
    const values = form.getValues();

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

  function onSubmit(values: z.infer<typeof editFormFormSchema>): void {
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

    editMutation.mutate(updatedForm, {
      onSuccess: () => {
        void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
      },
    });
  }

  const untranslatedQuestions = localQuestions.filter(
    (question) => !isQuestionTranslated(question, formData.defaultLanguage, languageCode)
  );

  const submit = (checkTranslations?: boolean): void => {
    if (checkTranslations && untranslatedQuestions.length > 0) {
      onOpenChange(true);
      return;
    }

    if (formRef.current) {
      formRef.current.dispatchEvent(new Event('submit', { cancelable: true, bubbles: true }));
    }
  };

  return (
    <Layout title={`${formData.code} - ${formData.name[formData.defaultLanguage]}`}>
      <Form {...form}>
        {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
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
              <Card className='pt-0 h-[calc(100vh)] overflow-hidden'>
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
          <EditFormFooter
            onSaveProgress={saveHandler}
            onSaveAndExit={() => {
              submit(true);
            }}
          />
          <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent
              className='min-w-[650px] min-h-[350px]'
              onInteractOutside={(e) => {
                e.preventDefault();
              }}
              onEscapeKeyDown={(e) => {
                e.preventDefault();
              }}>
              <DialogHeader>
                <DialogTitle className='mb-3.5'>Missing translations for {untranslatedQuestions.length} questions</DialogTitle>
                <DialogDescription className='mt-3.5 text-base text-slate-900'>
                  Please note that before publishing this form, all the questions must be translated in selected
                  language.
                </DialogDescription>
              </DialogHeader>

              <div className='grow-1'/>

              <DialogFooter className='sm:items-end'>
                <Button
                  onClick={() => {
                    submit();
                  }}
                  type='button'
                  variant='secondary'>
                  Save and exit anyway
                </Button>
                <DialogClose asChild>
                  <Button type='button' variant='default'>
                    Back to form editor
                  </Button>
                </DialogClose>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </form>
      </Form>
    </Layout>
  );
}
