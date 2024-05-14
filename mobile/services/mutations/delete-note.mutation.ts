import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { notesKeys } from "../queries.service";
import { Note } from "../../common/models/note";

export const useDeleteNote = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
  scopeId: string,
) => {
  const queryClient = useQueryClient();

  const getNotesQK = useMemo(
    () => notesKeys.notes(electionRoundId, pollingStationId, formId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: notesKeys.deleteNote(),
    scope: {
      id: scopeId,
    },
    onMutate: async (payload: Note) => {
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

      // Need to remove the CREATE mutation if the Note was not synched yet
      if (payload.isNotSynched) {
        const mutationsToRemove = queryClient
          .getMutationCache()
          .findAll({
            mutationKey: notesKeys.all, // matching ['notes', 'add' | 'update' | 'delete']
          })
          .filter((mutation) => {
            // @ts-ignore
            return mutation?.state?.variables?.id === payload.id;
          });

        for (const mutationToRemove of mutationsToRemove) {
          queryClient.getMutationCache().remove(mutationToRemove);
        }
      }

      // Return a context object with the snapshotted value
      return { prevNotes };
    },
    onError: (err) => {
      // TODO restore previous state
      console.log("🔴🔴🔴 ERROR IN DELETE NOTE MUTATION 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getNotesQK });
    },
  });
};
