import API from "../api";

export interface ERStatisticsModel {
  numberOfFormsSubmitted: number;
  numberOfQuestionsAnswered: number;
  numberOfQuickReports: number;
  numberOfNotes: number;
  numberOfAttachments: number;
  numberOfPollingStationsVisited: number;
}

export const getERStatistics = (electionRoundId: string): Promise<ERStatisticsModel> => {
  return API.get(`election-rounds/${electionRoundId}/statistics:my`).then((res) => res.data);
};
