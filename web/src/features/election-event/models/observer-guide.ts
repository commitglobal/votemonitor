export type ObserverGuideType = "Website" | "Document";
export interface ObserverGuide {
  id: string;
  title: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  urlValidityInSeconds?: number;
  websiteUrl?: string;
  guideType: ObserverGuideType;
  createdOn: string;
  createdBy: string;
}