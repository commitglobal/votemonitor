import { BaseQuestion, TranslatedString } from "@/common/types";
import i18n from "@/i18n";

export enum FormStatus {
    Drafted = 'Drafted',
    Published = 'Published',
    Obsolete = 'Obsolete',
}

export enum FormType {
    Opening = 'Opening',
    Voting = 'Voting',
    ClosingAndCounting = 'ClosingAndCounting',
    Other = 'Other',
}

export interface FormBase {
    id: string;
    formType: FormType;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    status: FormStatus;
    languages: string[];
    createdOn: string;
    lastModifiedOn: string | null;
}

export interface FormFull extends FormBase {
    questions: BaseQuestion[]
}

export function mapFormType(formType: FormType): string {
    switch (formType) {
        case FormType.Opening: return i18n.t('formType.opening');
        case FormType.Voting: return i18n.t('formType.voting');
        case FormType.ClosingAndCounting: return i18n.t('formType.closingAndCounting');
        case FormType.Other: return i18n.t('formType.other');
        default: return "Unknown";
    }
}

export interface NewFormRequest {
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description: TranslatedString;
    formType: FormType;
    languages: string[];
}
