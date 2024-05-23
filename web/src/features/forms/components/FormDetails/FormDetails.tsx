import { getTranslationOrDefault } from '@/common/types';
import Layout from '@/components/layout/Layout';
import PreviewQuestionFactory from '@/components/questionsEditor/preview/PreviewQuestionFactory';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Fieldset } from '@/components/ui/fieldset';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Route as FormDetailsRoute } from '@/routes/forms/$formId_.$languageCode';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useTranslation } from 'react-i18next';

import { useSuspenseQuery } from '@tanstack/react-query';
import { mapFormType } from '../../models/form';
import { formDetailsQueryOptions } from '../../queries';

export default function FormDetails() {
  const { formId, languageCode } = FormDetailsRoute.useParams();
  const formQuery = useSuspenseQuery(formDetailsQueryOptions(formId));
  const form = formQuery.data;
  
  const navigate = useNavigate();
  const { t } = useTranslation();
  const navigateToEdit = () => {
    navigate({ to: '/forms/$formId/edit', params: { formId } });
  };

  return (
    <Layout title={`${form.code} - ${form.name[form.defaultLanguage]}`}>
      <Tabs defaultValue='form-details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='form-details'>Form details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>
        <TabsContent value='form-details'>
          <Card className='pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='text-xl'>Form details</CardTitle>
                <Button onClick={navigateToEdit} variant='ghost-primary'>
                  <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                  <span className='text-base text-purple-900'>Edit</span>
                </Button>
              </div>
              <Separator />
            </CardHeader>
            <CardContent className='mt-6 grid grid-cols-5 gap-3'>
              <dl className='divide-y divide-gray-100 col-span-2'>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.code')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{form.code}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.name')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{form.name[form.defaultLanguage]}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.formType')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{mapFormType(form.formType)}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.defaultLanguage')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{form.defaultLanguage}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.languages')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    {form.languages.join(', ')}
                  </dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.status')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    <Badge className={'badge-' + form.status}>{form.status}</Badge>
                  </dd>
                </div>
              </dl>
              <dl className='col-span-3'>
                <div className='flex flex-col gap-1'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.description')}</dt>
                  <dd className='mt-1 text-sm leading-6 px-2 py-2 text-gray-700 sm:col-span-2 sm:mt-0 rounded-md border border-gray-200 min-h-[100px]'>
                    {getTranslationOrDefault(form.description, languageCode)}
                  </dd>
                </div>
              </dl>
            </CardContent>
            <CardFooter className='flex justify-between'></CardFooter>
          </Card>
        </TabsContent>
        <TabsContent value='questions'>
          <Card className='pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='text-xl'>Form questions</CardTitle>
                <Button onClick={navigateToEdit} variant='ghost-primary'>
                  <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                  <span className='text-base text-purple-900'>Edit</span>
                </Button>
              </div>
              <Separator />
            </CardHeader>
            <CardContent className='-mx-6 flex items-start justify-left px-6 sm:mx-0 sm:px-8'>
              <Fieldset className='grid gap-3 divide-y divide-gray-700'>
                {
                  form.questions.map(q => <PreviewQuestionFactory
                    languageCode={form.defaultLanguage}
                    question={q}
                    key={q.id} />)
                }
              </Fieldset>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
