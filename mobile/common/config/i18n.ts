import i18n, { ResourceLanguage } from "i18next";
import { initReactI18next } from "react-i18next";
import * as Localization from "expo-localization";
import ro from "../../assets/locales/ro/translations.json";
import en from "../../assets/locales/en/translations.json";
import bg from "../../assets/locales/bg/translations_BG.json";
import sr from "../../assets/locales/sr/translations_SR.json";
import pl from "../../assets/locales/pl/translations_PL.json";

import ka from "../../assets/locales/ka/translations_KA.json";
import hy from "../../assets/locales/hy/translations_HY.json";
import ru from "../../assets/locales/ru/translations_RU.json";
import az from "../../assets/locales/az/translations_AZ.json";

import { SECURE_STORAGE_KEYS } from "../constants";
import { getSecureStoreItem } from "../../helpers/SecureStoreWrapper";

const systemLocale =
  getSecureStoreItem(SECURE_STORAGE_KEYS.I18N_LANGUAGE) ||
  Localization.getLocales()?.[0]?.languageCode ||
  "en";

// handle RTL languages
const language = Localization.getLocales().find((lang) => lang.languageCode === systemLocale);
export const isRTL = language?.textDirection === "rtl";

i18n.use(initReactI18next).init<ResourceLanguage>({
  // default language app is currently the system locale or english
  lng: systemLocale || "en",
  fallbackLng: ["en", "ro", "pl", "bg", "sr", "ka", "hy", "ru", "az"],
  compatibilityJSON: "v3",
  supportedLngs: ["ro", "en", "pl", "bg", "sr", "ka", "hy", "ru", "az"],
  resources: {
    en,
    ro,
    pl,
    bg,
    sr,
    ka,
    hy,
    ru,
    az,
  },
  debug: true,
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
