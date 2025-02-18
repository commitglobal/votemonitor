import { BaseQuestion, FormBase, FormType, LanguagesTranslationStatus, TranslatedString } from "@/common/types";

export interface FormAccessModel {
  ngoId: string;
  name: string;
}

export interface NgoFormBase extends FormBase {
    formAccess: FormAccessModel[];
  }

export interface FormFull extends NgoFormBase {
    questions: BaseQuestion[]
}

export interface UpdateFormRequest {
    id: string;
    formType: FormType;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    languages: string[];
    icon?: string;
    questions: BaseQuestion[]
}


export interface NewFormRequest {
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    formType: FormType;
    languages: string[];
    icon?: string;
    questions: BaseQuestion[];
}


