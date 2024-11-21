import { BaseQuestion, FormType, LanguagesTranslationStatus, TranslatedString } from "@/common/types";

export enum FormStatus {
    Drafted = 'Drafted',
    Published = 'Published',
    Obsolete = 'Obsolete',
}

export const FormStatusList: FormStatus[] = [
    FormStatus.Drafted,
    FormStatus.Published,
    FormStatus.Obsolete
]

export interface FormAccessModel {
  ngoId: string;
  name: string;
}

export interface FormBase {
    id: string;
    formType: FormType;
    code: string;
    defaultLanguage: string;
    icon?: string;
    name: TranslatedString;
    description?: TranslatedString;
    status: FormStatus;
    languages: string[];
    lastModifiedOn: string;
    lastModifiedBy: string;
    numberOfQuestions: number;
    languagesTranslationStatus: LanguagesTranslationStatus;
    formAccess: FormAccessModel[]
}

export interface FormFull extends FormBase {
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
    description: TranslatedString;
    formType: FormType;
    languages: string[];
}
