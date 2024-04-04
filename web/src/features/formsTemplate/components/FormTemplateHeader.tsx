import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { FormTemplateFull } from '../models/formTemplate'
import { Input } from '@/components/ui/input';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import LanguagesMultiselect from '@/containers/LanguagesMultiselect';
import LanguageSelect from '@/containers/LanguageSelect';


export interface FormTemplateHeaderProps {
  formTemplate: FormTemplateFull;
}


const formTemplateHeaderSchema = z.object({
  code: z.string().min(2).max(255),
  name: z.string(),
  defaultLanguage: z.string().min(2),
  languages: z.array(z.string()).min(1),
});

export type FormTemplateHeaderValues = z.infer<typeof formTemplateHeaderSchema>;


function FormTemplateHeader({ formTemplate }: FormTemplateHeaderProps) {
  const { t } = useTranslation();

  const form = useForm<FormTemplateHeaderValues>({
    resolver: zodResolver(formTemplateHeaderSchema),
    defaultValues: {
      code: formTemplate.code,
      defaultLanguage: formTemplate.defaultLanguage,
      languages: formTemplate.languages,
      name: formTemplate.name[formTemplate.defaultLanguage]
    }

  });

  function onSubmit(data: FormTemplateHeaderValues) {
    alert(data);
  }



  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Template details</CardTitle>
      </CardHeader>
      <CardContent>
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
              
              <FormField
                control={form.control}
                name='defaultLanguage'
                render={({ field }) => (
                  <LanguageSelect
                    languageCode={field.value}
                    onSelect={field.onChange}
                  />
                )}
              />

              <FormField
                control={form.control}
                name="languages"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t('form-template.placeholder.languages')}</FormLabel>
                    <FormControl>
                      <LanguagesMultiselect
                        value={field.value}
                        defaultLanguage={formTemplate.defaultLanguage}
                        onChange={field.onChange}
                        placeholder={t('form-template.placeholder.languages')}
                        emptyIndicator={
                          <p className="text-center text-lg leading-10 text-gray-600 dark:text-gray-400">
                            no results found.
                          </p>
                        }
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  )
}

export default FormTemplateHeader
