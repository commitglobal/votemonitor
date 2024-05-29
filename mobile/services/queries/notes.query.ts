import { skipToken, useQuery } from "@tanstack/react-query";
import { notesKeys } from "../queries.service";
import { getNotesForPollingStation } from "../definitions.api";
import { Note } from "../../common/models/note";
import { useCallback } from "react";

export const useNotes = <TResult = Note[]>(
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
  return useNotes(
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

export const useNotesForFormId = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string,
) => {
  return useNotes(
    electionRoundId,
    pollingStationId,
    formId,
    useCallback(
      (data: Note[]) => {
        return data?.reduce((acc: Record<string, Note[]>, curr: Note) => {
          if (!acc[curr.questionId]) {
            acc[curr.questionId] = [];
          }
          acc[curr.questionId].push(curr);
          return acc;
        }, {});
      },
      [electionRoundId, pollingStationId, formId],
    ),
  );
};
