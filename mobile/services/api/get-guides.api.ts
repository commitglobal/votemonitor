import API from "../api";

export enum guideType {
  DOCUMENT = "Document",
  WEBSITE = "Website",
  TEXT = "Text",
}

export type Guide = {
  id: string;
  title: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
  websiteUrl: string;
  guideType: guideType;
  createdOn: Date;
  createdBy: string;
};

export const getGuides = ({ electionRoundId }: { electionRoundId: string }): Promise<Guide[]> => {
  return API.get(`election-rounds/${electionRoundId}/observer-guide`).then(
    (res) => res.data.guides,
  );
};
