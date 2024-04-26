import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteNote, DeleteNotePayload } from "../definitions.api";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { Note } from "../../common/models/note";

export const useDeleteNote = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
  id: string | undefined,
) => {
  const queryClient = useQueryClient();

  const getNotesQK = useMemo(
    () => pollingStationsKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: [`deleteNote_${id}`],
    mutationFn: async (payload: DeleteNotePayload) => {
      return deleteNote(payload);
    },
    onMutate: async (payload: DeleteNotePayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getNotesQK });

      // Snapshot the previous value
      const previousNotes = queryClient.getQueryData(getNotesQK);

      // Optimistically update to the new value (remove the note to delete)
      queryClient.setQueryData(getNotesQK, (prevNotes: Note[]) => {
        const index = prevNotes.findIndex((note) => note.id === payload.id);
        if (index === -1) return prevNotes;
        const updatedNotes = [...prevNotes.slice(0, index), ...prevNotes.slice(index + 1)];
        return updatedNotes;
      });

      // Return a context object with the snapshotted value
      return { previousNotes };
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
