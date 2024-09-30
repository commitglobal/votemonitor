export interface DataExport {
  exportedDataId: string;
  enqueuedAt: string;
}

export enum ExportStatus {
  Failed = "Failed",
  Started = "Started",
  Completed = "Completed"
}

export enum ExportedDataType {
  FormSubmissions = "FormSubmissions",
  QuickReports = "QuickReports",
  CitizenReports = "CitizenReports",
  IssueReports = "IssueReports",
  PollingStations = "PollingStations",
  Locations = "Locations",
}

export interface ExportedDataDetails {
  exportStatus: ExportStatus;
  exportedDataId: string;
  exportedDataType: ExportedDataType;
  fileName: null | string;
  startedAt: string;
  completedAt: null | string;
}