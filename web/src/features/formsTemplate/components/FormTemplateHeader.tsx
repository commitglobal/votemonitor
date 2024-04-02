import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { FormTemplateFull } from '../models/formTemplate'
import { Input } from '@/components/ui/input';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

export interface FormTemplateHeader {
  formTemplate: FormTemplateFull;
}


const formTemplateHeaderSchema = z.object({
  code: z.string().min(2).max(255),
  name: z.string(),
  defaultLanguage: z.string().min(2),
  languages: z.array(z.string())
});

export type FormTemplateHeaderValues = z.infer<typeof formTemplateHeaderSchema>;


function FormTemplateHeader(props: FormTemplateHeader) {
  const { t } = useTranslation();

  const form = useForm<FormTemplateHeaderValues>({
    resolver: zodResolver(formTemplateHeaderSchema)
  });

  function onSubmit(data: FormTemplateHeaderValues) {
    alert(data);
  }
  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className='grid gap-6 py-4 sm:grid-cols-2'>
          <FormField
            control={form.control}
            name='code'
            render={({ field }) => (
              <FormItem className='sm:col-span-2'>
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
        </div>
      </form>
    </Form>
  )
}


export default FormTemplateHeader
