import Layout from '@/components/layout/Layout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Route as FormDetailsRoute } from '@/routes/forms/$formId_.$languageCode';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useTranslation } from 'react-i18next';

import { isMultiSelectQuestion, isNumberQuestion, isRatingQuestion, isSingleSelectQuestion, isTextQuestion } from '@/common/guards';
import { FunctionComponent } from '@/common/types';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import PreviewDateQuestion from '@/components/questionsEditor/preview/PreviewDateQuestion';
import PreviewMultiSelectQuestion from '@/components/questionsEditor/preview/PreviewMultiSelectQuestion';
import PreviewNumberQuestion from '@/components/questionsEditor/preview/PreviewNumberQuestion';
import PreviewRatingQuestion from '@/components/questionsEditor/preview/PreviewRatingQuestion';
import PreviewSingleSelectQuestion from '@/components/questionsEditor/preview/PreviewSingleSelectQuestion';
import PreviewTextQuestion from '@/components/questionsEditor/preview/PreviewTextQuestion';
import { LanguageBadge } from '@/components/ui/language-badge';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getTranslationOrDefault, mapFormType } from '@/lib/utils';
import { useSuspenseQuery } from '@tanstack/react-query';
import { formDetailsQueryOptions } from '../../queries';
import { FormDetailsBreadcrumbs } from '../FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';

export default function FormDetails(): FunctionComponent {
  const { formId, languageCode } = FormDetailsRoute.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const formQuery = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));
  const form = formQuery.data;

  const navigate = useNavigate();
  const { t } = useTranslation();
  const navigateToEdit = (): void => {
    void navigate({ to: '/forms/$formId/edit', params: { formId } });
  };

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={form.code} formName={form.name[languageCode] ?? ''} />}
      title={`${form.code} - ${form.name[languageCode]}`}>
      <Tabs defaultValue='form-details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='form-details'>Form details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>
        <TabsContent value='form-details'>
          <Card className='pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='flex gap-1'>
                  <span className='text-xl'>Form details</span>
                  <LanguageBadge languageCode={languageCode} />
                </CardTitle>
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
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    {form.name[languageCode]}
                  </dd>
                </div>
                <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                  <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.formType')}</dt>
                  <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                    {mapFormType(form.formType)}
                  </dd>
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
          </Card>
        </TabsContent>
        <TabsContent value='questions'>
          <Card className='pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='flex gap-1'>
                  <span className='text-xl'>Form questions</span>
                  <LanguageBadge languageCode={languageCode} />
                </CardTitle>
                <Button onClick={navigateToEdit} variant='ghost-primary'>
                  <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                  <span className='text-base text-purple-900'>Edit</span>
                </Button>
              </div>
              <Separator />
            </CardHeader>
            <CardContent>
              <div className='w-1/2 flex-col space-y-6'>
                {form.questions.map((question) => (
                  <>
                    {
                      isTextQuestion(question) && (
                        <PreviewTextQuestion
                          questionId={question.id}
                          text={question.text[languageCode]}
                          helptext={question.helptext?.[languageCode]}
                          inputPlaceholder={question.inputPlaceholder?.[languageCode]}
                          code={question.code}
                        />)}

                    {
                      isNumberQuestion(question) && (
                        <PreviewNumberQuestion
                          questionId={question.id}
                          text={question.text[languageCode]}
                          helptext={question.helptext?.[languageCode]}
                          inputPlaceholder={question.inputPlaceholder?.[languageCode]}
                          code={question.code}
                        />)
                    }

                    {
                      isTextQuestion(question) && (
                        <PreviewDateQuestion
                          questionId={question.id}
                          text={question.text[languageCode]}
                          helptext={question.helptext?.[languageCode]}
                          code={question.code}
                        />)
                    }

                    {isRatingQuestion(question) && (
                      <PreviewRatingQuestion
                        questionId={question.id}
                        text={question.text[languageCode]}
                        helptext={question.helptext?.[languageCode]}
                        scale={question.scale}
                        upperLabel={question.upperLabel?.[languageCode]}
                        lowerLabel={question.lowerLabel?.[languageCode]}
                        code={question.code}
                      />)}

                    {isMultiSelectQuestion(question) && (
                      <PreviewMultiSelectQuestion
                        questionId={question.id}
                        text={question.text[languageCode]}
                        helptext={question.helptext?.[languageCode]}
                        options={question.options?.map(o => ({ optionId: o.id, isFreeText: o.isFreeText, text: o.text[languageCode] })) ?? []}
                        code={question.code}
                      />)}

                    {isSingleSelectQuestion(question) && (
                      <PreviewSingleSelectQuestion
                        questionId={question.id}
                        text={question.text[languageCode]}
                        helptext={question.helptext?.[languageCode]}
                        options={question.options?.map(o => ({ optionId: o.id, isFreeText: o.isFreeText, text: o.text[languageCode] })) ?? []}
                        code={question.code}
                      />)}
                  </>
                ))}

              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
