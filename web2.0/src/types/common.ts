import z from "zod";

export enum SortOrder {
  Asc = "Asc",
  Desc = "Desc",
}

export type PageResponse<T> = {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  items: T[];
};

export enum DataSource {
  Ngo = "ngo",
  Coalition = "coalition",
}

export interface NoteModel {
  text: string;
}

export interface AttachmentModel {
  fileName: string;
  filePath: string;
  mimeType: string;
  presignedUrl: string;
  uploadedFileName: string;
  urlValidityInSeconds: string;
}

export const ZTranslatedString = z.record(z.string(), z.string());
export type TranslatedString = z.infer<typeof ZTranslatedString>;
