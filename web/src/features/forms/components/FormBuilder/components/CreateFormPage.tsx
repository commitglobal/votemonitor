import { authApi } from '@/common/auth-api';
import { ZFormType } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Textarea } from '@/components/ui/textarea';
import LanguageSelect from '@/containers/LanguageSelect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FormBase, NewFormRequest } from '@/features/forms/models/form';
import { formsKeys } from '@/features/forms/queries';
import { cn, mapFormType, newTranslatedString } from '@/lib/utils';
import { queryClient } from '@/main';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { FC } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

export const CreateFormPage: FC = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore((s) => s.isMonitoringNgoForCitizenReporting);

  const newFormFormSchema = z.object({
    code: z.string().nonempty('Form code is required'),
    name: z.string().nonempty('Form name is required'),
    description: z.string().optional(),
    defaultLanguage: z.string().nonempty(),
    formType: ZFormType.catch(ZFormType.Values.Opening),
  });

  const form = useForm<z.infer<typeof newFormFormSchema>>({
    resolver: zodResolver(newFormFormSchema),
  });

  function onSubmit(values: z.infer<typeof newFormFormSchema>) {
    const name = newTranslatedString([values.defaultLanguage], values.defaultLanguage, values.name);
    const description = newTranslatedString([values.defaultLanguage], values.defaultLanguage, values.description ?? '');

    const newForm: NewFormRequest = {
      ...values,
      description,
      name,
      languages: [values.defaultLanguage],
    };

    newFormMutation.mutate({ electionRoundId: currentElectionRoundId, newForm });
  }

  const newFormMutation = useMutation({
    mutationFn: ({ electionRoundId, newForm }: { electionRoundId: string; newForm: NewFormRequest }) => {
      return authApi.post<FormBase>(`/election-rounds/${electionRoundId}/forms`, newForm);
    },

    onSuccess: ({ data: form }) => {
      queryClient.invalidateQueries({ queryKey: formsKeys.all(currentElectionRoundId) });
      navigate({ to: `/forms/$formId/edit`, params: { formId: form.id }, search: { tab: 'questions' } });
    },
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <Tabs className='flex flex-col flex-1' defaultValue='form-details'>
          <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
            <TabsTrigger
              value='form-details'
              className={cn({
                'border-b-4 border-red-400': form.getFieldState('name').invalid || form.getFieldState('code').invalid,
              })}>
              Form details
            </TabsTrigger>
            <TabsTrigger value='questions' disabled>
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
                <div className='md:inline-flex md:space-x-6 w-full'>
                  <div className='space-y-4 md:w-1/2'>
                    <FormField
                      control={form.control}
                      name='name'
                      render={({ field, fieldState }) => (
                        <FormItem>
                          <FormLabel>{t('form.field.name')}</FormLabel>
                          <Input placeholder={t('form.placeholder.name')} {...field} {...fieldState} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name='code'
                      render={({ field, fieldState }) => (
                        <FormItem>
                          <FormLabel>{t('form.field.code')}</FormLabel>
                          <Input placeholder={t('form.placeholder.code')} {...field} {...fieldState} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name='formType'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{t('form.field.formType')}</FormLabel>
                          <Select onValueChange={field.onChange} defaultValue={field.value}>
                            <FormControl>
                              <SelectTrigger>
                                <SelectValue placeholder='Select a form type' />
                              </SelectTrigger>
                            </FormControl>
                            <SelectContent>
                              <SelectItem value={ZFormType.Values.Opening}>
                                {mapFormType(ZFormType.Values.Opening)}
                              </SelectItem>
                              <SelectItem value={ZFormType.Values.Voting}>
                                {mapFormType(ZFormType.Values.Voting)}
                              </SelectItem>
                              <SelectItem value={ZFormType.Values.ClosingAndCounting}>
                                {mapFormType(ZFormType.Values.ClosingAndCounting)}
                              </SelectItem>
                              {isMonitoringNgoForCitizenReporting && (
                                <SelectItem value={ZFormType.Values.CitizenReporting}>
                                  {mapFormType(ZFormType.Values.CitizenReporting)}
                                </SelectItem>
                              )}
                              <SelectItem value={ZFormType.Values.IncidentReporting}>
                                {mapFormType(ZFormType.Values.IncidentReporting)}
                              </SelectItem>
                              <SelectItem value={ZFormType.Values.Other}>
                                {mapFormType(ZFormType.Values.Other)}
                              </SelectItem>
                            </SelectContent>
                          </Select>
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name='defaultLanguage'
                      render={({ field, fieldState }) => (
                        <FormItem>
                          <FormLabel>{t('form.field.defaultLanguage')}</FormLabel>
                          <LanguageSelect languageCode={field.value} onLanguageSelected={field.onChange} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                  <div className='md:w-1/2'>
                    <FormField
                      control={form.control}
                      name='description'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{t('form.field.description')}</FormLabel>
                          <Textarea
                            resizable={false}
                            rows={10}
                            cols={100}
                            {...field}
                            placeholder={t('form.placeholder.description')}
                          />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
        <footer className='fixed left-0 bottom-0 h-[64px] w-full bg-white'>
          <div className='container flex items-center justify-end h-full gap-4'>
            <Button title='Next' type='submit' variant='default' disabled={!form.formState.isValid}>
              Next
            </Button>
          </div>
        </footer>
      </form>
    </Form>
  );
};
