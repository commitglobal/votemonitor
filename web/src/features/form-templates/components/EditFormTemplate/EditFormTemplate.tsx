import { useLoaderData, useNavigate } from '@tanstack/react-router';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { authApi } from '@/common/auth-api';
import { useMutation } from '@tanstack/react-query';
import { useToast } from '@/components/ui/use-toast';
import { FormTemplateFull, FormTemplateType } from '../../models/formTemplate';
import { Route as FormTemplatesListRoute } from '@/routes/form-templates/'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useTranslation } from 'react-i18next';
import LanguageSelect from '@/containers/LanguageSelect';
import { Textarea } from '@/components/ui/textarea';

export default function EditFormTemplate() {
  const navigate = useNavigate({ from: FormTemplatesListRoute.fullPath })
  const { t } = useTranslation();
  const formTemplate: FormTemplateFull = useLoaderData({ strict: false });
  const { toast } = useToast();

  const editFormTemplateFormSchema = z.object({
    code: z.string(),
    name: z.string(),
    description: z.string(),
    defaultLanguage: z.string(),
    formType: z.enum([FormTemplateType.Opening, FormTemplateType.Voting, FormTemplateType.ClosingAndCounting]).catch(FormTemplateType.Opening)
  });

  const form = useForm<z.infer<typeof editFormTemplateFormSchema>>({
    resolver: zodResolver(editFormTemplateFormSchema),
    defaultValues: {
      code: formTemplate.code,
      name: formTemplate.name[formTemplate.defaultLanguage],
      description: formTemplate.description[formTemplate.defaultLanguage],
      formType: formTemplate.formTemplateType,
      defaultLanguage: formTemplate.defaultLanguage
    },
  });

  function onSubmit(values: z.infer<typeof editFormTemplateFormSchema>) {
    const updatedFormTemplate: FormTemplateFull = {
      ...formTemplate
    };

    editMutation.mutate(updatedFormTemplate);
  }

  const deleteMutation = useMutation({
    mutationFn: (formTemplateId: string) => {
      return authApi.delete<void>(`/form-templates/${formTemplateId}`);
    },
    onSuccess: () => {
      navigate({ search: (prev) => (prev) });
    },
  });

  const editMutation = useMutation({
    mutationFn: (obj: FormTemplateFull) => {

      return authApi.post<void>(
        `/form-templates/${formTemplate.id}`,
        obj
      );
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Observer successfully updated',
      });
    },
  });

  const { setValue } = form;

  return (
    <Layout title={`Edit ${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage]}`}>
      <Card className=' pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit form template</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
              <div className='grid gap-6 py-4 grid-cols-2'>
                <div>
                  <FormField
                    control={form.control}
                    name='code'
                    render={({ field }) => (
                      <FormItem >
                        <FormLabel>{t('form-template.field.code')}</FormLabel>
                        <FormControl>
                          <Input placeholder={t('form-template.placeholder.code')} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                      <FormItem className='sm:col-span-2'>
                        <FormLabel>{t('form-template.field.name')}</FormLabel>
                        <FormControl>
                          <Input placeholder={t('form-template.placeholder.name')} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name='defaultLanguage'
                    render={({ field }) => (
                      <FormItem className='flex flex-col sm:col-span-2'>
                        <FormLabel>{t('form-template.field.defaultLanguage')}</FormLabel>
                        <FormControl className='py-2'>
                          <LanguageSelect
                            languageCode={field.value}
                            onSelect={field.onChange}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
                <div>
                  <FormField
                    control={form.control}
                    name='description'
                    render={({ field }) => (
                      <FormItem className='sm:col-span-2'>
                        <FormLabel>{t('form-template.field.description')}</FormLabel>
                        <FormControl>
                          <Textarea placeholder={t('form-template.placeholder.description')} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>
            </form>
          </Form>
        </CardContent>
        <CardFooter className='flex justify-between'></CardFooter>
      </Card>
    </Layout>
  );
}
