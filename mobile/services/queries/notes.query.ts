import { skipToken, useQuery } from "@tanstack/react-query";
import { notesKeys } from "../queries.service";
import { getNotesForPollingStation } from "../definitions.api";
import { Note } from "../../common/models/note";
import { useCallback } from "react";

export const useNotesForPollingStation = <TResult = Note[]>(
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string,
  select?: (data: Note[]) => TResult,
) => {
  return useQuery({
    queryKey: notesKeys.notes(electionRoundId, pollingStationId, formId),
    queryFn:
      electionRoundId && pollingStationId && formId
        ? () => getNotesForPollingStation(electionRoundId, pollingStationId, formId)
        : skipToken,
    select,
  });
};

export const useNotesForQuestionId = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string,
  questionId: string,
) => {
  return useNotesForPollingStation(
    electionRoundId,
    pollingStationId,
    formId,
    useCallback(
      (data: Note[]) => {
        return data.filter((note) => note.questionId === questionId);
      },
      [electionRoundId, pollingStationId, formId, questionId],
    ),
  );
};
