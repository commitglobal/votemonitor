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

const DETECTION_OPTIONS = {
  order: ['localStorage', 'navigator'],
  caches: ['localStorage'],
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    debug: true,
    defaultNS: 'translation',
    contextSeparator: '|',
    detection: DETECTION_OPTIONS,
    interpolation: {
      escapeValue: false,
    },
    resources,
    load: 'languageOnly',
    fallbackLng: 'en',

    backend: {
      loadPath: `/locales/{{lng}}.json`, //path of the languages
    },
  });

export default i18n;
