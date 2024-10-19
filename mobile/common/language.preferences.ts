import { getSecureStoreItem, setSecureStoreItem } from "../helpers/SecureStoreWrapper";

const formLanguagePreferenceKey = (formId: string) => `form_${formId}_language_preference`;

export const setFormLanguagePreference = async ({
  formId,
  language,
}: {
  formId: string;
  language: string;
}) => {
  return setSecureStoreItem(formLanguagePreferenceKey(formId), language);
};

export const getFormLanguagePreference = async ({ formId }: { formId: string }) => {
  return getSecureStoreItem(formLanguagePreferenceKey(formId));
};
