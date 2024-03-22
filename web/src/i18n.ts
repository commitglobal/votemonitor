import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import en from './locales/en.json';

const resources = {
  en: {
    translation: en,
  },
} as const;

export type Dict = typeof resources.en;

i18n.use(initReactI18next).init({
  debug: true,
  lng: 'en',
  defaultNS: 'translation',
  contextSeparator: '|',
  interpolation: {
    escapeValue: false,
  },
  resources,
});

export default i18n;
