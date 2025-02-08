import { BaseQuestion, FormType, LanguagesTranslationStatus, TranslatedString } from '@/common/types';

export enum FormTemplateStatus {
  Drafted = 'Drafted',
  Published = 'Published',
  Obsolete = 'Obsolete',
}

export const FormStatusList: FormTemplateStatus[] = [
  FormTemplateStatus.Drafted,
  FormTemplateStatus.Published,
  FormTemplateStatus.Obsolete,
];

export interface FormTemplateBase {
  id: string;
  formType: FormType;
  code: string;
  defaultLanguage: string;
  icon?: string;
  name: TranslatedString;
  description?: TranslatedString;
  isFormOwner: boolean;
  status: FormTemplateStatus;
  languages: string[];
  lastModifiedOn: string;
  lastModifiedBy: string;
  numberOfQuestions: number;
  languagesTranslationStatus: LanguagesTranslationStatus;
}

export interface FormTemplateFull extends FormTemplateBase {
  questions: BaseQuestion[];
}

export interface UpdateFormTemplateRequest {
  id: string;
  formType: FormType;
  code: string;
  defaultLanguage: string;
  name: TranslatedString;
  description?: TranslatedString;
  languages: string[];
  icon?: string;
  questions: BaseQuestion[];
}

export interface NewFormTemplateRequest {
  code: string;
  defaultLanguage: string;
  name: TranslatedString;
  description?: TranslatedString;
  formType: FormType;
  languages: string[];
  icon?: string;
  questions: BaseQuestion[];
}
