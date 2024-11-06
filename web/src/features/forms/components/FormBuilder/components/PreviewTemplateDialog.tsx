import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { useFormTemplateDetails } from '@/features/forms/queries';
import { useTranslation } from 'react-i18next';
import { create } from 'zustand';

import {
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from '@/common/guards';
import PreviewDateQuestion from '@/components/questionsEditor/preview/PreviewDateQuestion';
import PreviewMultiSelectQuestion from '@/components/questionsEditor/preview/PreviewMultiSelectQuestion';
import PreviewNumberQuestion from '@/components/questionsEditor/preview/PreviewNumberQuestion';
import PreviewRatingQuestion from '@/components/questionsEditor/preview/PreviewRatingQuestion';
import PreviewSingleSelectQuestion from '@/components/questionsEditor/preview/PreviewSingleSelectQuestion';
import PreviewTextQuestion from '@/components/questionsEditor/preview/PreviewTextQuestion';
import { Button } from '@/components/ui/button';
import { FC } from 'react';

export interface AddTranslationsDialogPropsProps {
  isOpen: boolean;
  templateId: string;
  languageCode: string;
  trigger: (templateId: string, languageCode: string) => void;
  dismiss: VoidFunction;
}

export const usePreviewTemplateDialog = create<AddTranslationsDialogPropsProps>((set) => ({
  isOpen: false,
  templateId: '',
  languageCode: '',
  trigger: (templateId: string, languageCode: string) => set({ templateId, languageCode, isOpen: true }),
  dismiss: () => set({ isOpen: false }),
}));

interface TemplateDetailProps {
  name: string;
  content: string | undefined;
  noContentErrorMessage?: string;
}

const TemplateDetail: FC<TemplateDetailProps> = ({ name, content, noContentErrorMessage }) => {
  return (
    <div className='flex flex-col gap-1'>
      <p className='font-bold text-gray-700'>{name}</p>
      <p className='font-normal text-gray-900'>{content ?? noContentErrorMessage}</p>
    </div>
  );
};

function PreviewTemplateDialog({}) {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form.template' });
  const { isOpen, templateId, languageCode, trigger, dismiss } = usePreviewTemplateDialog();
  const { data } = useFormTemplateDetails(templateId);
  console.log(data);

  const onOpenChange = (open: boolean) => {
    if (open) trigger(templateId, languageCode);
    else dismiss();
  };

  const filteredLanguages = data?.languages && data?.languages.filter((language) => language !== data.defaultLanguage);

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='min-w-[700px]  max-h-[650px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogTitle className='mb-3.5'>{languageCode ? `Preview ${data?.name[languageCode]}` : ''}</DialogTitle>
        <div className='flex flex-col gap-3 overflow-y-auto max-h-[500px]'>
          <TemplateDetail
            name={t('formDescription')}
            content={data?.description?.[languageCode]}
            noContentErrorMessage={t('formDescriptionErr')}
          />
          <TemplateDetail
            name={t('formBaseLanguage')}
            content={data?.defaultLanguage}
            noContentErrorMessage={t('formBaseLanguageErr')}
          />

          {filteredLanguages && filteredLanguages?.length > 0 && (
            <TemplateDetail name={t('formOtherLanguages')} content={filteredLanguages?.join(', ')} />
          )}

          {data?.questions?.length == 0 && (
            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>{t('formQuestions')}</p>
              <p className='font-normal text-gray-900'>{t('formQuestionsErr')}</p>
            </div>
          )}

          {data?.questions && data?.questions?.length > 0 && (
            <div className='flex flex-col gap-1 mt-2'>
              <p className='font-bold text-gray-700'>{`${t('formQuestions')}: ${data.questions.length}`}</p>

              {data?.questions?.map((question) => (
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
          )}
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
              Cancel
            </Button>
          </DialogClose>
          <Button title={t('useTemplate')} type='submit' className='px-6'>
            {t('useTemplate')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default PreviewTemplateDialog;
