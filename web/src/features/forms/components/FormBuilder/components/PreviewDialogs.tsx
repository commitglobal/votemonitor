import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { useTranslation } from 'react-i18next';

import { Button } from '@/components/ui/button';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import {
  useCreateFormFromForm,
  useCreateFormFromFormDialog,
  useCreateFormFromTemplate,
  usePreviewTemplateDialog,
} from '@/features/forms/hooks';
import { useSuspenseQuery } from '@tanstack/react-query';
import { FC, ReactNode, useMemo } from 'react';
import { useformTemplateDetails } from '@/features/form-templates/queries';
import { FormQuestionsPreview } from '@/components/FormQuestionsPreview/FormQuestionsPreview';

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

interface PreviewTemplateDialogProps {
  isOpen: boolean;
  title: string;
  confirmBtn: ReactNode;
  details: ReactNode;
  questionsList: ReactNode;
  onOpenChange: (open: boolean) => void;
}

const PreviewFormOrTemplateDialog: FC<PreviewTemplateDialogProps> = (props) => {
  const { isOpen, onOpenChange, title, confirmBtn, details, questionsList } = props;
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
        <DialogTitle className='mb-3.5'>{title}</DialogTitle>
        <div className='flex flex-col gap-3 overflow-y-auto max-h-[500px]'>
          {details}
          {questionsList}
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
              Cancel
            </Button>
          </DialogClose>
          {confirmBtn}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export const PreviewTemplateDialog = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form.template' });
  const { isOpen, id, languageCode, trigger, dismiss } = usePreviewTemplateDialog();
  const { data: formTemplate } = useformTemplateDetails(id);
  const { createForm } = useCreateFormFromTemplate();

  const onOpenChange = (open: boolean) => {
    if (open) trigger(id, languageCode);
    else dismiss();
  };

  const filteredLanguages = useMemo(
    () => formTemplate?.languages && formTemplate?.languages.filter((language) => language !== languageCode),
    [formTemplate?.languages, languageCode]
  );

  return (
    <PreviewFormOrTemplateDialog
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      title={languageCode ? `Preview ${formTemplate?.name[languageCode]}` : ''}
      details={
        <>
          <TemplateDetail
            name={t('formDescription')}
            content={formTemplate?.description?.[languageCode]}
            noContentErrorMessage={t('formDescriptionErr')}
          />
          <TemplateDetail
            name={t('formBaseLanguage')}
            content={languageCode}
            noContentErrorMessage={t('formBaseLanguageErr')}
          />

          {filteredLanguages && filteredLanguages?.length > 0 && (
            <TemplateDetail name={t('formOtherLanguages')} content={filteredLanguages?.join(', ')} />
          )}
        </>
      }
      questionsList={
        <FormQuestionsPreview
          questions={formTemplate?.questions}
          languageCode={languageCode}
          title={t('formQuestions')}
          noContentMessage={t('formQuestionsErr')}
        />
      }
      confirmBtn={
        <Button
          onClick={() => createForm({ templateId: id, languageCode })}
          title={t('useTemplate')}
          type='submit'
          className='px-6'>
          {t('useTemplate')}
        </Button>
      }
    />
  );
};

export const PreviewFormReuseDialog = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form.template' });
  const { isOpen, id: formId, languageCode, trigger, dismiss } = useCreateFormFromFormDialog();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const { data: formData } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));
  const { createForm } = useCreateFormFromForm();

  const onOpenChange = (open: boolean) => {
    if (open) trigger(formId, languageCode);
    else dismiss();
  };

  const filteredLanguages = formData?.languages && formData?.languages.filter((language) => language !== languageCode);

  return (
    <PreviewFormOrTemplateDialog
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      title={languageCode ? `Preview ${formData?.name[languageCode]}` : ''}
      details={
        <>
          <TemplateDetail
            name={t('formDescription')}
            content={formData?.description?.[languageCode]}
            noContentErrorMessage={t('formDescriptionErr')}
          />
          <TemplateDetail
            name={t('formBaseLanguage')}
            content={languageCode}
            noContentErrorMessage={t('formBaseLanguageErr')}
          />

          {filteredLanguages && filteredLanguages?.length > 0 && (
            <TemplateDetail name={t('formOtherLanguages')} content={filteredLanguages?.join(', ')} />
          )}
        </>
      }
      questionsList={
        <FormQuestionsPreview
          questions={formData?.questions}
          languageCode={languageCode}
          title={t('formQuestions')}
          noContentMessage={t('formQuestionsErr')}
        />
      }
      confirmBtn={
        <Button
          onClick={() => createForm({ formId, languageCode })}
          title={t('useTemplate')}
          type='submit'
          className='px-6'>
          {t('useTemplate')}
        </Button>
      }
    />
  );
};
