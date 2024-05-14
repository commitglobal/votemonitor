import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { notesKeys } from "../queries.service";
import { Note } from "../../common/models/note";
import { UpsertNotePayload, upsertNote } from "../definitions.api";

export const useUpdateNote = (
  electionRoundId: string,
  pollingStationId: string,
  formId: string,
  scopeId: string,
) => {
  const queryClient = useQueryClient();

  // this is the GET notes key - we need it in order to invalidate that query after updating the note
  const getNotesQK = useMemo(
    () => notesKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: notesKeys.updateNote(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: UpsertNotePayload) => {
      return upsertNote(payload);
    },
    onMutate: async (payload: UpsertNotePayload) => {
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
      console.log("🔴🔴🔴 ERROR IN EDIT NOTE MUTATION 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
