interface HistogramEntry {
    bucket: string;
    value: number;
}

interface ObserversStats {
    activeObservers: number;
    pendingObservers: number;
    suspendedObservers: number;
    totalNumberOfObservers: number;
}

interface PollingStationsStats {
    totalNumberOfPollingStations: number;
    numberOfVisitedPollingStations: number;
}

interface MonitoringStats {
    observersStats: ObserversStats;
    numberOfObserversOnTheField: number;
    pollingStationsStats: PollingStationsStats;
    minutesMonitoring: number;
    formsHistogram: HistogramEntry[];
    questionsHistogram: HistogramEntry[];
    flaggedAnswersHistogram: HistogramEntry[];
    quickReportsHistogram: HistogramEntry[];
}