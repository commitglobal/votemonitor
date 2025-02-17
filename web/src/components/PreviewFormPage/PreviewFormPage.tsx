import Layout from '@/components/layout/Layout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { PencilIcon } from '@heroicons/react/24/outline';
import { ReactNode } from '@tanstack/react-router';
import { useTranslation } from 'react-i18next';

import {
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from '@/common/guards';
import { FormStatus, FunctionComponent } from '@/common/types';
import PreviewDateQuestion from '@/components/FormQuestionsPreview/PreviewDateQuestion';
import PreviewMultiSelectQuestion from '@/components/FormQuestionsPreview/PreviewMultiSelectQuestion';
import PreviewNumberQuestion from '@/components/FormQuestionsPreview/PreviewNumberQuestion';
import PreviewRatingQuestion from '@/components/FormQuestionsPreview/PreviewRatingQuestion';
import PreviewSingleSelectQuestion from '@/components/FormQuestionsPreview/PreviewSingleSelectQuestion';
import PreviewTextQuestion from '@/components/FormQuestionsPreview/PreviewTextQuestion';
import { LanguageBadge } from '@/components/ui/language-badge';
import { FormTemplateFull } from '@/features/form-templates/models';
import { FormFull } from '@/features/forms/models';
import { getTranslationOrDefault, isNotNilOrWhitespace, mapFormType } from '@/lib/utils';
import FormStatusBadge from '../FormStatusBadge/FormStatusBadge';

export interface PreviewFormPageProps {
  form: FormFull | FormTemplateFull;
  languageCode: string;
  onNavigateToEdit: () => void;
  breadcrumbs?: ReactNode;
  hideEditButton?: boolean;
}
export default function PreviewFormPage({
  form,
  languageCode,
  breadcrumbs,
  onNavigateToEdit,
  hideEditButton = false,
}: PreviewFormPageProps): FunctionComponent {
  const { t } = useTranslation();

  return (
    <Tabs defaultValue='form-details'>
      <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
        <TabsTrigger value='form-details'>Form details</TabsTrigger>
        <TabsTrigger value='questions'>Questions</TabsTrigger>
      </TabsList>
      <TabsContent value='form-details'>
        <Card className='pt-0'>
          <CardHeader className='flex gap-2 flex-column'>
            <div className='flex flex-row items-center justify-between'>
              <CardTitle className='flex gap-1'>
                <span className='text-xl'>Form details</span>
                <LanguageBadge languageCode={languageCode} />
              </CardTitle>
              {!hideEditButton && (
                <Button
                  onClick={onNavigateToEdit}
                  variant='ghost-primary'
                  disabled={form.status !== FormStatus.Drafted}>
                  <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                  <span className='text-base text-purple-900'>Edit</span>
                </Button>
              )}
            </div>
            <Separator />
          </CardHeader>
          <CardContent className='grid grid-cols-5 gap-3 mt-6'>
            <dl className='col-span-2 divide-y divide-gray-100'>
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
                <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.icon')}</dt>
                <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                  {isNotNilOrWhitespace(form.icon) ? (
                    <div dangerouslySetInnerHTML={{ __html: form.icon ?? '' }}></div>
                  ) : (
                    <>-</>
                  )}
                </dd>
              </div>
              <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.defaultLanguage')}</dt>
                <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                  <LanguageBadge languageCode={form.defaultLanguage} variant={'unstyled'} displayMode='native' />
                </dd>
              </div>
              <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.languages')}</dt>
                <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                  {form.languages.map((l) => (
                    <LanguageBadge languageCode={l} variant={'unstyled'} displayMode='native' />
                  ))}
                </dd>
              </div>
              <div className='px-4 py-6 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-0'>
                <dt className='text-sm font-medium leading-6 text-gray-900'>{t('form.field.status')}</dt>
                <dd className='mt-1 text-sm leading-6 text-gray-700 sm:col-span-2 sm:mt-0'>
                  <FormStatusBadge status={form.status} />
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
          <CardHeader className='flex gap-2 flex-column'>
            <div className='flex flex-row items-center justify-between'>
              <CardTitle className='flex gap-1'>
                <span className='text-xl'>Form questions</span>
                <LanguageBadge languageCode={languageCode} />
              </CardTitle>
              <Button onClick={onNavigateToEdit} variant='ghost-primary' disabled={form.status !== FormStatus.Drafted}>
                <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                <span className='text-base text-purple-900'>Edit</span>
              </Button>
            </div>
            <Separator />
          </CardHeader>
          <CardContent>
            <div className='flex-col w-1/2 space-y-6'>
              {form.questions.map((question) => (
                <>
                  {isTextQuestion(question) && (
                    <PreviewTextQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      inputPlaceholder={question.inputPlaceholder?.[languageCode]}
                      code={question.code}
                    />
                  )}

                  {isNumberQuestion(question) && (
                    <PreviewNumberQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      inputPlaceholder={question.inputPlaceholder?.[languageCode]}
                      code={question.code}
                    />
                  )}

                  {isDateQuestion(question) && (
                    <PreviewDateQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      code={question.code}
                    />
                  )}

                  {isRatingQuestion(question) && (
                    <PreviewRatingQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      scale={question.scale}
                      upperLabel={question.upperLabel?.[languageCode]}
                      lowerLabel={question.lowerLabel?.[languageCode]}
                      code={question.code}
                    />
                  )}

                  {isMultiSelectQuestion(question) && (
                    <PreviewMultiSelectQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      options={
                        question.options?.map((o) => ({
                          optionId: o.id,
                          isFreeText: o.isFreeText,
                          text: o.text[languageCode],
                        })) ?? []
                      }
                      code={question.code}
                    />
                  )}

                  {isSingleSelectQuestion(question) && (
                    <PreviewSingleSelectQuestion
                      questionId={question.id}
                      text={question.text[languageCode]}
                      helptext={question.helptext?.[languageCode]}
                      options={
                        question.options?.map((o) => ({
                          optionId: o.id,
                          isFreeText: o.isFreeText,
                          text: o.text[languageCode],
                        })) ?? []
                      }
                      code={question.code}
                    />
                  )}
                </>
              ))}
            </div>
          </CardContent>
        </Card>
      </TabsContent>
    </Tabs>
  );
}
