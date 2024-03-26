import { BaseQuestion, TranslatedString } from "@/common/types";

export enum FormTemplateStatus {
    Drafted = 'Drafted',
    Published = 'Published',
}

export interface FormTemplateBase {
    id: string;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    status: FormTemplateStatus;
    createdOn: string;
    lastModifiedOn: string | null;
    languages: string[];
}


export interface FormTemplateFull extends FormTemplateBase {
    questions: BaseQuestion[]
}
