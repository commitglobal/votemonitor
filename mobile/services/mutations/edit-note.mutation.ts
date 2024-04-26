import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateNote, UpdateNotePayload } from "../definitions.api";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { Note } from "../../common/models/note";

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
    onMutate: async (payload: UpdateNotePayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getNotesQK });

      // Snapshot the previous value
      const previousNotes: Note[] | undefined = queryClient.getQueryData(getNotesQK);

      if (!previousNotes) {
        return;
      }

      // Optimistically update to the new value
      const updatedNotes = previousNotes.map((note) => {
        if (note.id === payload.id) {
          // update the text for the edited note
          return { ...note, text: payload.text };
        }
        return note;
      });

      // Optimistically update to the new value
      queryClient.setQueryData(getNotesQK, [...updatedNotes]);

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
