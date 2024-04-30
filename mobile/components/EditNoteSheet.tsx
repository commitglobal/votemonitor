import React, { Dispatch, SetStateAction } from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import Button from "./Button";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import { useAddNoteMutation } from "../services/mutations/add-note.mutation";
import { Keyboard, TextInput } from "react-native";
import OptionsSheet from "./OptionsSheet";
import { useUpdateNote } from "../services/mutations/edit-note.mutation";
import { useDeleteNote } from "../services/mutations/delete-note.mutation";

const EditNoteSheet = ({
  open,
  setOpen,
  selectedNote,
  electionRoundId,
  pollingStationId,
  formId,
  questionId,
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
    selectedNote?.id,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  const { mutate: deleteNote } = useDeleteNote(
    electionRoundId,
    pollingStationId,
    formId,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  return (
    <OptionsSheet open={open} setOpen={setOpen} moveOnKeyboardChange={true}>
      <XStack justifyContent="space-between" alignItems="center">
        <Typography preset="heading">Edit note</Typography>
        <Typography
          preset="body2"
          color="$red10"
          paddingVertical="$xxxs"
          paddingLeft="$xs"
          pressStyle={{ opacity: 0.5 }}
          // onPress={onDelete}
        >
          Delete note
        </Typography>
      </XStack>
      <Controller
        key={selectedNote?.id + "_edit_note"}
        name={"noteEditedText"}
        control={control}
        render={({ field: { value, onChange } }) => {
          return <Input type="textarea" value={value} onChangeText={onChange} />;
        }}
      />
    </OptionsSheet>
  );
};

export default EditNoteSheet;
