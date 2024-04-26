import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { addNote, NotePayload } from "../definitions.api";

export const useAddNoteMutation = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) => {
  const queryClient = useQueryClient();

  // this is the GET notes key - we need it in order to invalidate that query after adding the new note
  const getNotesQK = useMemo(
    () => pollingStationsKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: ["addNote"],
    mutationFn: async (payload: NotePayload) => {
      return addNote(payload);
    },
    onMutate: async (_payload: NotePayload) => {
      await queryClient.cancelQueries({ queryKey: getNotesQK });
      //TODO: optimistic updates
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
