import Layout from '@/components/layout/Layout';
import { FC, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FormBuilderChoice } from './FormBuilderChoice';

enum FormBuilderScreens {
  Start,
  Scratch,
  Template,
  Reuse,
}

interface StartScreenProps {
  setCurrentScreen: (screen: FormBuilderScreens) => void;
}

const StartScreen: FC<StartScreenProps> = ({ setCurrentScreen }) => {
  return (
    <div className='grid grid-cols-1 lg:grid-cols-3 gap-8'>
      <FormBuilderChoice type='scratch' onClick={() => setCurrentScreen(FormBuilderScreens.Scratch)} />
      <FormBuilderChoice type='template' />
      <FormBuilderChoice type='reuse' />
    </div>
  );
};

export const FormBuilder: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });

  const [currentScreen, setCurrentScreen] = useState<FormBuilderScreens>(FormBuilderScreens.Start);

  return (
    <Layout title={t('title')} subtitle={t('subtitle')}>
      {currentScreen === FormBuilderScreens.Start && <StartScreen setCurrentScreen={setCurrentScreen} />}
      {currentScreen === FormBuilderScreens.Scratch && <div>Scratch screen</div>}
    </Layout>
  );
};
