import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { addNote, NotePayload } from "../definitions.api";
import { Note } from "../../common/models/note";
import * as Crypto from "expo-crypto";

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
    onMutate: async (payload: NotePayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getNotesQK });

      // Snapshot the previous value
      const previousNotes = queryClient.getQueryData(getNotesQK);

      // Optimistically update to the new value
      queryClient.setQueryData(getNotesQK, (old: Note[]) => [
        ...old,
        { id: Crypto.randomUUID(), ...payload },
      ]);

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
