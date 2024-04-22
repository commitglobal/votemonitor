import Layout from '@/components/layout/Layout';
import PreviewQuestionFactory from '@/components/questionsEditor/preview/PreviewQuestionFactory';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Fieldset } from '@/components/ui/fieldset';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Route as FormTemplateDetailsRoute } from '@/routes/form-templates/$formTemplateId_.$languageCode';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useLoaderData, useNavigate } from '@tanstack/react-router';
import { useTranslation } from 'react-i18next';
import { FormTemplateFull, mapFormTemplateType } from '../../models/formTemplate';

export default function FormTemplateDetails() {
  const formTemplate: FormTemplateFull = useLoaderData({ strict: false });
  const { formTemplateId, languageCode } = FormTemplateDetailsRoute.useParams();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const navigateToEdit = () => {
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId: formTemplate.id } });
  };

  return (
    <Layout title={`${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage]}`}>
      <Tabs defaultValue='form-details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='form-details'>Template form details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>
        <TabsContent value='form-details'>
          <Card className='pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='text-xl'>Template form details</CardTitle>
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
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.code')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{formTemplate.code}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.name')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{formTemplate.name[formTemplate.defaultLanguage]}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.formTemplateType')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{mapFormTemplateType(formTemplate.formTemplateType)}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.defaultLanguage')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>{formTemplate.defaultLanguage}</dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.languages')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    {formTemplate.languages.join(', ')}
                  </dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.status')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    <Badge className={'badge-' + formTemplate.status}>{formTemplate.status}</Badge>
                  </dd>
                </div>
              </dl>
              <dl className='col-span-3'>
                <div className='flex flex-col gap-1'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form-template.field.description')}</dt>
                  <dd className='mt-1 text-sm leading-6 px-2 py-2 text-gray-700 sm:col-span-2 sm:mt-0 rounded-md border border-gray-200 min-h-[100px]'>
                    {formTemplate.description[languageCode]}
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
                <CardTitle className='text-xl'>Template form questions</CardTitle>
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
                  formTemplate.questions.map(q => <PreviewQuestionFactory
                    languageCode={formTemplate.defaultLanguage}
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
