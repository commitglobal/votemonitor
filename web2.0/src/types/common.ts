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
  type: "Note";
  text: string;
}

export interface AttachmentModel {
  type: "Attachment";
  fileName: string;
  filePath: string;
  mimeType: string;
  presignedUrl: string;
  uploadedFileName: string;
  urlValidityInSeconds: string;
}
