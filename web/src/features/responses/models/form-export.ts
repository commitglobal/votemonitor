export interface FormSubmissionsExport {
  exportedDataId: string;
  enqueuedAt: string;
}

export enum ExportStatus {
    Failed = "Failed",
    Started = "Started",
    Completed = "Completed"
}

export interface FormSubmissionsExportedDataDetails {
  exportStatus: ExportStatus;
  exportedDataId: string;
  fileName: null | string;
  startedAt: string;
  completedAt: null | string; 
}