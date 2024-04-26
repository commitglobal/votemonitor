import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateNote, UpdateNotePayload } from "../definitions.api";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";

export const useUpdateNote = (
  electionRoundId: string,
  pollingStationId: string,
  formId: string,
  id: string,
) => {
  const queryClient = useQueryClient();

  // this is the GET notes key - we need it in order to invalidate that query after updating the note
  const getNotesQK = useMemo(
    () => pollingStationsKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: [`updateNote_${id}`],
    mutationFn: async (payload: UpdateNotePayload) => {
      return updateNote(payload);
    },
    onMutate: async (_payload: UpdateNotePayload) => {
      await queryClient.cancelQueries({ queryKey: getNotesQK });
      // TODO: optimistic updates
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
