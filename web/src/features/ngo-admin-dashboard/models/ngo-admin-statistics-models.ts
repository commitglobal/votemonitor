export interface HistogramEntry {
  bucket: string;
  value: number;
}

export interface ObserversStats {
  activeObservers: number;
  pendingObservers: number;
  suspendedObservers: number;
  totalNumberOfObservers: number;
}
export interface VisitedPollingStationLevelStats {
  path: string;
  numberOfPollingStations: number;
  numberOfVisitedPollingStations: number;
  numberOfIncidentReports: number;
  numberOfQuickReports: number;
  numberOfFormSubmissions: number;
  minutesMonitoring: number;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  activeObservers: number;
  level: number;
  coveragePercentage: number;
}

export interface MonitoringNgoStats {
  observersStats: ObserversStats;
  totalStats?: VisitedPollingStationLevelStats;
  level1Stats: VisitedPollingStationLevelStats[];
  level2Stats: VisitedPollingStationLevelStats[];
  level3Stats: VisitedPollingStationLevelStats[];
  level4Stats: VisitedPollingStationLevelStats[];
  level5Stats: VisitedPollingStationLevelStats[];
  formsHistogram: HistogramEntry[];
  questionsHistogram: HistogramEntry[];
  flaggedAnswersHistogram: HistogramEntry[];
  quickReportsHistogram: HistogramEntry[];
  citizenReportsHistogram: HistogramEntry[];
  incidentReportsHistogram: HistogramEntry[];
}
