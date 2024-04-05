import i18n, { ResourceLanguage } from "i18next";
import { initReactI18next } from "react-i18next";
import * as Localization from "expo-localization";
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

// handle RTL languages
const systemLocale = Localization.getLocales()[0];
export const isRTL = systemLocale?.textDirection === "rtl";

export default i18n;
