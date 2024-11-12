import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ClipboardDocumentListIcon, DocumentPlusIcon, DocumentTextIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import { FC } from 'react';
import { useTranslation } from 'react-i18next';

interface FormBuilderChoiceIconProps {
  type: FormBuilderChoice;
}

const FormBuilderChoiceIcon: FC<FormBuilderChoiceIconProps> = ({ type }) => {
  const classes = 'stroke-purple-900 h-16 w-16 md:h-32 md:w-32';
  switch (type) {
    case 'scratch':
      return <DocumentPlusIcon className={classes} />;

    case 'template':
      return <DocumentTextIcon className={classes} />;

    case 'reuse':
      return <ClipboardDocumentListIcon className={classes} />;

    default:
      return <></>;
  }
};

type FormBuilderChoice = 'scratch' | 'template' | 'reuse';

interface FormBuilderChoiceProps {
  type: FormBuilderChoice;
}
const FormBuilderChoice: FC<FormBuilderChoiceProps> = ({ type }) => {
  const { t } = useTranslation('translation', { keyPrefix: `electionEvent.form.${type}` });

  return (
    <Card>
      <CardHeader>
        <CardTitle>{t('title')}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className='bg-gray-50 flex flex-col gap-6 justify-center items-center rounded-lg p-6'>
          <FormBuilderChoiceIcon type={type} />
          <p className='text-center'>{t('description')}</p>

          <Link to={`/forms/new/${type}`}>
            <Button
              title={t('buttonText')}
              className='flex gap-2 text-purple-900 bg-background hover:bg-purple-50 hover:text-purple-500'
              variant='outline'>
              {t('buttonText')}
            </Button>
          </Link>
        </div>
      </CardContent>
    </Card>
  );
};

export const FormBuilderScreenStart: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });

  return (
    <Layout
      title={t('title')}
      subtitle={t('subtitle')}
      enableBackButton
      enableBreadcrumbs={false}
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}>
      <div className='grid grid-cols-1 lg:grid-cols-3 gap-8'>
        <FormBuilderChoice type='scratch' />
        <FormBuilderChoice type='template' />
        <FormBuilderChoice type='reuse' />
      </div>
    </Layout>
  );
};
