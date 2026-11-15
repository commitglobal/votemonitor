import React, { Dispatch, SetStateAction } from "react";
import { useAddNoteMutation } from "../services/mutations/add-note.mutation";
import { Keyboard } from "react-native";
import * as Crypto from "expo-crypto";
import { useTranslation } from "react-i18next";
import NoteForm from "./NoteForm";

const AddNoteSheetContent = ({
  setAddingNote,
  submissionId,
  questionId,
  electionRoundId = "",
  setIsOptionsSheetOpen,
}: {
  setAddingNote: Dispatch<SetStateAction<boolean>>;
  submissionId: string;
  questionId: string;
  electionRoundId: string | undefined;
  setIsOptionsSheetOpen: Dispatch<SetStateAction<boolean>>;
}) => {
  const { t } = useTranslation("polling_station_form_wizard");

  const { mutate: addNote } = useAddNoteMutation(
    electionRoundId,
    submissionId,
    `Note_${electionRoundId}_${submissionId}_${questionId}`,
  );

  const onSubmitNote = (text: string) => {
    addNote({
      electionRoundId,
      submissionId,
      id: Crypto.randomUUID(),
      text,
      questionId,
      lastUpdatedAt: new Date().toISOString(),
    });
    Keyboard.dismiss();
    setIsOptionsSheetOpen(false);
    setAddingNote(false);
  };

  return (
    <NoteForm
      key={questionId + "_note"}
      heading={t("notes.add.heading")}
      onSave={onSubmitNote}
      onCancel={() => setAddingNote(false)}
    />
  );
};

export default AddNoteSheetContent;
