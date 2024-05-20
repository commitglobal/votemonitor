import i18n, { ResourceLanguage } from "i18next";
import { initReactI18next } from "react-i18next";
import * as Localization from "expo-localization";
import ro from "../../assets/locales/ro/translations.json";
import en from "../../assets/locales/en/translations.json";

const systemLocale = Localization.getLocales()[0];
// handle RTL languages
export const isRTL = systemLocale?.textDirection === "rtl";

i18n.use(initReactI18next).init<ResourceLanguage>({
  //default language app is currently the system locale or english
  lng: systemLocale.languageCode || "en",
  fallbackLng: ["en", "ro"],
  compatibilityJSON: "v3",
  supportedLngs: ["ro", "en"],
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
