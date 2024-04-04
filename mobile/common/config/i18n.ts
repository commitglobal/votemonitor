import i18n, { ResourceLanguage } from "i18next";
import { initReactI18next } from "react-i18next";
import ro from "../../assets/locales/ro/translations.json";
import en from "../../assets/locales/en/translations.json";

i18n.use(initReactI18next).init<ResourceLanguage>({
  lng: "ro",
  fallbackLng: "en",
  compatibilityJSON: "v3",
  resources: {
    en,
    ro,
  },
  debug: true,
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
