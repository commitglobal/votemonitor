import { authApi } from '@/common/auth-api';
import type { DataSources } from '@/common/types';
import type { VisitedPollingStationLevelStats } from '@/features/ngo-admin-dashboard/models/ngo-admin-statistics-models';

export interface MonitoringObserverStats {
  totalStats?: VisitedPollingStationLevelStats;
  level1Stats: VisitedPollingStationLevelStats[];
  level2Stats: VisitedPollingStationLevelStats[];
  level3Stats: VisitedPollingStationLevelStats[];
  level4Stats: VisitedPollingStationLevelStats[];
  level5Stats: VisitedPollingStationLevelStats[];
  numberOfFormsSubmitted: number;
  numberOfQuestionsAnswered: number;
  numberOfFlaggedAnswers: number;
  numberOfQuickReports: number;
  numberOfIncidentReports: number;
  numberOfNotes: number;
  numberOfAttachments: number;
  numberOfPollingStationsVisited: number;
  minutesMonitoring: number;
}

export async function getMonitoringObserverStatistics(
  electionRoundId: string,
  monitoringObserverId: string,
  dataSource: DataSources
): Promise<MonitoringObserverStats> {
  const response = await authApi.get<MonitoringObserverStats>(
    `/election-rounds/${electionRoundId}/statistics/observers/${monitoringObserverId}?dataSource=${dataSource}`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch monitoring observer statistics');
  }

  return response.data;
}
