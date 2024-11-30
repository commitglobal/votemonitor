export enum GuidePageType {
  Observer = 'observer-type',
  Citizen = 'citizen-type',
}

export enum GuideType {
  Website = 'Website',
  Document = 'Document',
  Text = 'Text',
}
export interface GuideAccessModel {
  ngoId: string;
  name: string;
}
export interface GuideModel {
  id: string;
  title: string;
  fileName: string;
  mimeType: string;
  guideType: GuideType;
  text: string;
  websiteUrl: string;
  createdOn: string; // ISO 8601 date string for DateTime
  createdBy: string;
  isGuideOwner: boolean;
  presignedUrl: string;
  urlValidityInSeconds: number;
  filePath: string;
  uploadedFileName: string;
  guideAccess: GuideAccessModel[];
}
