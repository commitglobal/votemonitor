import Layout from '@/components/layout/Layout';
import i18n from '@/i18n';
import { FC } from 'react';
import { FormBuilderChoice } from './FormBuilderChoice';

export const FormBuilder: FC = () => {
  return (
    <Layout title={i18n.t('electionEvent.form.title')} subtitle={i18n.t('electionEvent.form.subtitle')}>
      <div className='grid grid-cols-1 lg:grid-cols-3 gap-8'>
        <FormBuilderChoice type='scratch' />
        <FormBuilderChoice type='template' />
        <FormBuilderChoice type='reuse' />
      </div>
    </Layout>
  );
};
