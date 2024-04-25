import React, { Dispatch, SetStateAction } from "react";
import { Dialog } from "./Dialog";
import { Typography } from "./Typography";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import { XStack } from "tamagui";
import Button from "./Button";
import { Keyboard } from "react-native";
import { useAddNoteMutation } from "../services/mutations/add-note.mutation";

interface AddNoteModalProps {
  open: boolean;
  setOpen: Dispatch<SetStateAction<boolean>>;
  pollingStationId: string;
  formId: string;
  questionId: string;
  electionRoundId: string | undefined;
}

const AddNoteModal: React.FC<AddNoteModalProps> = ({
  open,
  setOpen,
  pollingStationId,
  formId,
  questionId,
  electionRoundId,
}) => {
  const { control, handleSubmit } = useForm({});

  const { mutate: addNote } = useAddNoteMutation(electionRoundId, pollingStationId, formId);

  const onSubmitNote = (note: any) => {
    const notePayload = {
      pollingStationId,
      text: note.note_text,
      formId,
      questionId,
    };

    addNote({ electionRoundId, ...notePayload });
    Keyboard.dismiss();
    setOpen(false);
  };

  return (
    <Dialog
      open={open}
      header={<Typography preset="heading">Add a note</Typography>}
      content={
        <Controller
          key={questionId + "_note"}
          name={"note_text"}
          control={control}
          render={({ field: { value: noteValue, onChange: onNoteChange } }) => {
            return (
              <Input
                type="textarea"
                placeholder="Add any relevant notes to this question."
                value={noteValue}
                onChangeText={onNoteChange}
              />
            );
          }}
        />
      }
      footer={
        <XStack gap="$md">
          <Button preset="chromeless" onPress={() => setOpen(false)}>
            Cancel
          </Button>
          <Button flex={1} onPress={handleSubmit(onSubmitNote)}>
            Save
          </Button>
        </XStack>
      }
    />
  );
};

export default AddNoteModal;
