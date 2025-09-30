import { skipToken, useQuery } from "@tanstack/react-query";
import { notesKeys } from "../queries.service";
import { getNotesForSubmission } from "../definitions.api";
import { Note } from "../../common/models/note";
import { useCallback } from "react";

export const useNotes = <TResult = Note[]>(
  electionRoundId: string | undefined,
  submissionId: string | undefined,
  select?: (data: Note[]) => TResult,
) => {
  return useQuery({
    queryKey: notesKeys.notes(electionRoundId, submissionId),
    queryFn:
      electionRoundId && submissionId
        ? () => getNotesForSubmission(electionRoundId, submissionId)
        : skipToken,
    select,
  });
};

export const useNotesForQuestionId = (
  electionRoundId: string | undefined,
  submissionId: string,
  questionId: string,
) => {
  return useNotes(
    electionRoundId,
    submissionId,
    useCallback(
      (data: Note[]) => {
        return data
          .filter((note) => note.questionId === questionId)
          .sort((a, b) => +new Date(b.lastUpdatedAt) - +new Date(a.lastUpdatedAt)); // added unary '+' operator to avoid ts error that the right-hand side of an arithmetic operation must be of type 'any', 'number', 'bigint' or an enum type
      },
      [electionRoundId, submissionId, questionId],
    ),
  );
};

export const useNotesForSubmission = (
  electionRoundId: string | undefined,
  submissionId: string | undefined,
) => {
  return useNotes(
    electionRoundId,
    submissionId,
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
      [electionRoundId, submissionId],
    ),
  );
};
