import Layout from '@/components/layout/Layout';
import { FC } from 'react';
import { useTranslation } from 'react-i18next';

export const FormBuilderScreenTemplate: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });

  return (
    <Layout title={t('template.title')} subtitle={t('template.description')}>
      <div className='grid grid-cols-1 lg:grid-cols-3 gap-8'></div>
    </Layout>
  );
};
