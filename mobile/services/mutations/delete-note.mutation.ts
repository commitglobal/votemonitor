import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteNote, DeleteNotePayload } from "../definitions.api";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { Note } from "../../common/models/note";

export const useDeleteNote = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
  scopeId: string,
) => {
  const queryClient = useQueryClient();

  const getNotesQK = useMemo(
    () => pollingStationsKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.deleteNote(),
    mutationFn: async (payload: DeleteNotePayload) => {
      return deleteNote(payload);
    },
    scope: {
      id: scopeId,
    },
    onMutate: async (payload: DeleteNotePayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getNotesQK });

      // Snapshot the previous value
      const prevNotes = queryClient.getQueryData(getNotesQK);

      // Optimistically update to the new value (remove the note to delete)
      queryClient.setQueryData(getNotesQK, (prevNotes: Note[]) => {
        const updatedNotes = prevNotes.filter((note) => note.id !== payload.id);
        return updatedNotes;
      });

      // Return a context object with the snapshotted value
      return { prevNotes };
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR IN DELETE NOTE MUTATION 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
