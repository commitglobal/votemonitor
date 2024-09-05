


export enum SubmissionType {
  FormSubmission = 'FormSubmission',
  QuickReport = 'QuickReport',
  CitizenReport = 'CitizenReport',
}

type AttachmentsAndNotesData = {
  submissionType: SubmissionType;
  submissionId: string;
  timeSubmitted: string;
  questionId: string;
}

export interface Note extends AttachmentsAndNotesData {
  type: "Note";
  text: string;
}

export interface Attachment extends AttachmentsAndNotesData {
  type: "Attachment";
  fileName: string;
  filePath: string;
  mimeType: string;
  presignedUrl: string;
  uploadedFileName: string;
  urlValidityInSeconds: string;
}
