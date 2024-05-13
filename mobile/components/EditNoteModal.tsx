import React from "react";
import { Dialog } from "./Dialog";
import { Typography } from "./Typography";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import { XStack } from "tamagui";
import Button from "./Button";
import { Note } from "../common/models/note";
import { useUpdateNote } from "../services/mutations/edit-note.mutation";
import { useDeleteNote } from "../services/mutations/delete-note.mutation";

const EditNoteModal = ({
  selectedNote,
  setSelectedNote,
  electionRoundId,
  pollingStationId,
  formId,
  questionId,
}: {
  selectedNote: Note | null;
  setSelectedNote: React.Dispatch<React.SetStateAction<Note | null>>;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
}) => {
  const { control, handleSubmit } = useForm({
    defaultValues: {
      noteEditedText: selectedNote?.text,
    },
  });

  const { mutate: updateNote } = useUpdateNote(
    electionRoundId,
    pollingStationId,
    formId,
    selectedNote!.id,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  const { mutate: deleteNote } = useDeleteNote(
    electionRoundId,
    pollingStationId,
    formId,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  const onDelete = () => {
    // delete note
    if (selectedNote) {
      deleteNote(selectedNote);
      // close dialog
      setSelectedNote(null);
    }
  };

  const onSubmit = (formData: any) => {
    const updateNotePayload = {
      electionRoundId,
      pollingStationId,
      formId,
      questionId,
      id: selectedNote!.id,
      text: formData.noteEditedText,
    };
    // update the note
    updateNote(updateNotePayload);
    // close dialog
    setSelectedNote(null);
  };

  return (
    <Dialog
      open={!!selectedNote}
      header={
        <XStack justifyContent="space-between" alignItems="center">
          <Typography preset="heading">Edit note</Typography>
          <Typography
            preset="body2"
            color="$red10"
            paddingVertical="$xxxs"
            paddingLeft="$xs"
            pressStyle={{ opacity: 0.5 }}
            onPress={onDelete}
          >
            Delete note
          </Typography>
        </XStack>
      }
      content={
        <Controller
          key={selectedNote?.id + "_edit_note"}
          name={"noteEditedText"}
          control={control}
          render={({ field: { value, onChange } }) => {
            return <Input type="textarea" value={value} onChangeText={onChange} />;
          }}
        />
      }
      footer={
        <XStack gap="$md">
          <Button preset="chromeless" onPress={() => setSelectedNote(null)}>
            Cancel
          </Button>
          <Button flex={1} onPress={handleSubmit(onSubmit)}>
            Save
          </Button>
        </XStack>
      }
    />
  );
};

export default EditNoteModal;
