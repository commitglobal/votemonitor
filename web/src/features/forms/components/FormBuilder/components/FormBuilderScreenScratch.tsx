import Layout from '@/components/layout/Layout';
import { FC } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { EditFormType } from '../../EditForm/EditForm';
import { CreateFormPage } from './CreateFormPage';

export const FormBuilderScreenScratch: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });
  const form = useForm<EditFormType>({
    defaultValues: {
      formId: undefined,
      code: undefined,
      languageCode: undefined,
      defaultLanguage: undefined,
      languages: undefined,
      name: undefined,
      description: undefined,
      formType: undefined,
      questions: undefined,
    },
    mode: 'all',
  });
  return (
    <Layout title={t('scratch.title')} subtitle={t('scratch.description')}>
      <CreateFormPage />
    </Layout>
  );
};
