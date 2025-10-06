import z from "zod";
import type { TranslatedString } from "./common";
import { FormType } from "./form";

export enum FormSubmissionFollowUpStatus {
  NotApplicable = "NotApplicable",
  NeedsFollowUp = "NeedsFollowUp",
  Resolved = "Resolved",
}

export interface FormSubmissionModel {
  submissionId: string;
  email: string;
  observerName: string;
  ngoName: string;
  phoneNumber: string;
  formCode: string;
  formType: FormType;
  formName: TranslatedString;
  defaultLanguage: string;
  languages: string[];
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  pollingStationId: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  mediaFilesCount: number;
  notesCount: number;
  number: string;
  isOwnObserver: boolean;
  tags: string[];
  timeSubmitted: string;
  followUpStatus: FormSubmissionFollowUpStatus;
}

export interface FormSubmissionByFormModel {
  formId: string;
  formCode: string;
  formType: FormType;
  formName: TranslatedString;
  defaultLanguage: string;
  numberOfSubmissions: number;
  numberOfFlaggedAnswers: number;
  numberOfNotes: number;
  numberOfMediaFiles: number;
}

export interface FormSubmissionDetailedModel {
  submissionId: string;
  email: string;
  observerName: string;
  ngoName: string;
  phoneNumber: string;
  formCode: string;
  formType: FormType;
  formName: TranslatedString;
  defaultLanguage: string;
  languages: string[];
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  pollingStationId: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  mediaFilesCount: number;
  notesCount: number;
  number: string;
  isOwnObserver: boolean;
  tags: string[];
  timeSubmitted: string;
  followUpStatus: FormSubmissionFollowUpStatus;
}
export enum QuestionsAnswered {
  None = "None",
  Some = "Some",
  All = "All",
}
export const formSubmissionsSearchSchema = z.object({
  searchText: z.string().optional(),
  formTypeFilter: z.enum(FormType).optional(),
  level1Filter: z.string().optional(),
  level2Filter: z.string().optional(),
  level3Filter: z.string().optional(),
  level4Filter: z.string().optional(),
  level5Filter: z.string().optional(),
  pollingStationNumberFilter: z.string().optional(),
  hasFlaggedAnswers: z.string().optional(),
  monitoringObserverId: z.string().optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  followUpStatus: z.enum(FormSubmissionFollowUpStatus).optional(),

  questionsAnswered: z.enum(QuestionsAnswered).optional(),
  hasNotes: z.string().optional(),
  hasAttachments: z.string().optional(),
  formId: z.string().optional(),

  submissionsFromDate: z.coerce.date().optional(),
  submissionsToDate: z.coerce.date().optional(),
  coalitionMemberId: z.string().optional(),
});

export type FormSubmissionsSearch = z.infer<typeof formSubmissionsSearchSchema>;
