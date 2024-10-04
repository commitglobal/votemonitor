import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import detector from 'i18next-browser-languagedetector';
import en from './locales/en.json';

const resources = {
  en: {
    translation: en,
  },
} as const;

export type Dict = typeof resources.en;

i18n
  .use(detector)
  .use(initReactI18next)
  .init({
    debug: true,
    defaultNS: 'translation',
    contextSeparator: '|',
    interpolation: {
      escapeValue: false,
    },
    resources,
  });

export default i18n;