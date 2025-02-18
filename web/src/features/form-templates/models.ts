import { BaseQuestion, FormBase, FormType, TranslatedString } from '@/common/types';

export interface FormTemplateFull extends FormBase {
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
