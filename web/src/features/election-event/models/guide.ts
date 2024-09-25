export enum GuidePageType {
  Observer = 'observer-type',
  Citizen = 'citizen-type',
}

export enum GuideType {
  Website = 'Website',
  Document = 'Document',
  Text = 'Text',
}

export interface GuideModel {
  id: string;
  title: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  urlValidityInSeconds?: number;
  websiteUrl?: string;
  text?: string;
  guideType: GuideType;
  createdOn: string;
  createdBy: string;
}
