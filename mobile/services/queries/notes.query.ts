import { skipToken, useQuery } from "@tanstack/react-query";
import { notesKeys } from "../queries.service";
import { getNotesForPollingStation } from "../definitions.api";
import { Note } from "../../common/models/note";

const mapAPINotesToQuestionNote = (apiNotes: Note[] | undefined) => {
  return apiNotes?.reduce((acc: Record<string, Note[]>, curr: Note) => {
    if (!acc[curr.questionId]) {
      acc[curr.questionId] = [];
    }
    acc[curr.questionId].push(curr);
    return acc;
  }, {});
};

export const useNotesForPollingStation = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) => {
  return useQuery({
    queryKey: notesKeys.notes(electionRoundId, pollingStationId, formId),
    queryFn:
      electionRoundId && pollingStationId && formId
        ? () => getNotesForPollingStation(electionRoundId, pollingStationId, formId)
        : skipToken,
    select: mapAPINotesToQuestionNote,
  });
};
