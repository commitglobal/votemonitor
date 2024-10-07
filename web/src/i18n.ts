import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import LanguageDetector from 'i18next-browser-languagedetector';
import en from './locales/en.json';
import ro from './locales/ro.json';

const resources = {
  en: {
    translation: en,
  },
  ro: {
    translation: ro,
  },
} as const;

export type Dict = typeof resources.en;

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    debug: true,
    lng: 'en',
    supportedLngs: ['en', 'ro'],
    defaultNS: 'translation',
    contextSeparator: '|',
    interpolation: {
      escapeValue: false,
    },
    resources,
    load: 'languageOnly',
    fallbackLng: 'en',
  });

export default i18n;
