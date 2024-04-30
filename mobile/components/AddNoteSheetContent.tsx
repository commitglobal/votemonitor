import React, { Dispatch, SetStateAction } from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import Button from "./Button";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import { useAddNoteMutation } from "../services/mutations/add-note.mutation";
import { Keyboard, TextInput } from "react-native";

const AddNoteSheetContent = ({
  setAddingNote,
  pollingStationId,
  formId,
  questionId,
  electionRoundId = "",
  setIsOptionsSheetOpen,
}: {
  setAddingNote: Dispatch<SetStateAction<boolean>>;
  pollingStationId: string;
  formId: string;
  questionId: string;
  electionRoundId: string | undefined;
  setIsOptionsSheetOpen: Dispatch<SetStateAction<boolean>>;
}) => {
  const { control, handleSubmit, reset } = useForm({
    defaultValues: {
      noteText: "",
    },
  });

  const { mutate: addNote } = useAddNoteMutation(
    electionRoundId,
    pollingStationId,
    formId,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  const onSubmitNote = (note: any) => {
    const notePayload = {
      pollingStationId,
      text: note.noteText,
      formId,
      questionId,
    };

    addNote({ electionRoundId, ...notePayload });
    Keyboard.dismiss();
    setIsOptionsSheetOpen(false);
    setAddingNote(false);
    reset();
  };

  return (
    <YStack marginHorizontal={12} gap="$md">
      <Typography preset="heading">Add a note</Typography>

      <Controller
        key={questionId + "_note"}
        name={"noteText"}
        control={control}
        render={({ field: { value: noteValue, onChange: onNoteChange } }) => {
          return (
            <YStack height={150}>
              <Input
                type="textarea"
                placeholder="Add any relevant notes to this question."
                value={noteValue}
                height={150}
                onChangeText={onNoteChange}
              />
            </YStack>
          );
        }}
      />

      <XStack gap="$md">
        <Button preset="chromeless" onPress={() => setAddingNote(false)}>
          Cancel
        </Button>
        <Button flex={1} onPress={handleSubmit(onSubmitNote)}>
          Save
        </Button>
      </XStack>
    </YStack>
  );
};

export default AddNoteSheetContent;
