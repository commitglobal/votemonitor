import Layout from '@/components/layout/Layout';
import { FC } from 'react';
import { useTranslation } from 'react-i18next';
import { CreateFormPage } from './CreateFormPage';

export const FormBuilderScreenScratch: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });
  return (
    <Layout title={t('scratch.title')} subtitle={t('scratch.description')}>
      <CreateFormPage />
    </Layout>
  );
};
