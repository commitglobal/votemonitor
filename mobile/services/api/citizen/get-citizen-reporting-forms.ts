import API from "../../api";

export interface ICitizenReportingForm {
  id: string;
  name?: string;
  description?: string;
  formType?: string;
  code?: string;
  status?: string;
  defaultLanguage?: string;
  languages?: string[];
  numberOfQuestions?: number;
  createdOn?: string;
  lastModifiedOn?: string;
  languagesTranslationStatus?: Record<string, unknown>;
  questions?: {
    $questionType?: string;
    id?: string;
    code?: string;
    text?: Record<string, string>;
    helptext?: Record<string, string>;
    displayLogic?: {
      parentQuestionId?: string;
      condition?: string;
      value?: string;
    };
  }[];
}

interface IGetCitizenReportingFormsResponse {
  electionRoundId: string;
  version: string;
  forms: ICitizenReportingForm[];
}

export const getCitizenReportingForms = (
  electionRoundId: string,
): Promise<IGetCitizenReportingFormsResponse> => {
  return API.get(`/election-rounds/${electionRoundId}/citizen-reporting-forms`).then(
    (res) => res.data,
  );
};
