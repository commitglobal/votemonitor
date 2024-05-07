import * as SecureStore from "expo-secure-store";

const formLanguagePreferenceKey = (formId: string) => `form_${formId}_language_preference`;

export const setFormLanguagePreference = async ({
  formId,
  language,
}: {
  formId: string;
  language: string;
}) => {
  return SecureStore.setItem(formLanguagePreferenceKey(formId), language);
};

export const getFormLanguagePreference = async ({ formId }: { formId: string }) => {
  return SecureStore.getItem(formLanguagePreferenceKey(formId));
};
