import { BaseQuestion, TranslatedString } from "@/common/types";
import i18n from "@/i18n";

export enum FormTemplateStatus {
    Drafted = 'Drafted',
    Published = 'Published',
}

export enum FormTemplateType {
    Opening = 'Opening',
    Voting = 'Voting',
    ClosingAndCounting = 'ClosingAndCounting',
    Other = 'Other',
}

export interface FormTemplateBase {
    id: string;
    formTemplateType: FormTemplateType;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    status: FormTemplateStatus;
    languages: string[];
    createdOn: string;
    lastModifiedOn: string | null;
}


export interface FormTemplateFull extends FormTemplateBase {
    questions: BaseQuestion[]
}


export function mapFormTemplateType(formType: FormTemplateType): string {
    switch (formType) {
        case FormTemplateType.Opening: return i18n.t('formTemplateType.opening');
        case FormTemplateType.Voting: return i18n.t('formTemplateType.voting');
        case FormTemplateType.ClosingAndCounting: return i18n.t('formTemplateType.closingAndCounting');
        case FormTemplateType.Other: return i18n.t('formTemplateType.other');
        default: return "Unknown";
    }
}

export interface NewFormTemplateRequest {
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description: TranslatedString;
    formTemplateType: FormTemplateType;
    languages: string[];
}
