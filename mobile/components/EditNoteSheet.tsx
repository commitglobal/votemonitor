import React, { useEffect } from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Sheet, SheetProps, XStack, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Note } from "../common/models/note";
import { Typography } from "./Typography";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import Button from "./Button";
import { useDeleteNote } from "../services/mutations/delete-note.mutation";
import { useUpdateNote } from "../services/mutations/edit-note.mutation";

interface EditNoteSheetProps extends SheetProps {
  selectedNote: Note | null;
  setSelectedNote: React.Dispatch<React.SetStateAction<Note | null>>;
  questionId: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
}

const EditNoteSheet = (props: EditNoteSheetProps) => {
  const { selectedNote, setSelectedNote, questionId, electionRoundId, pollingStationId, formId } =
    props;
  const insets = useSafeAreaInsets();

  const { control, handleSubmit, setValue } = useForm({
    defaultValues: {
      noteEditedText: selectedNote?.text,
    },
  });

  useEffect(() => {
    setValue("noteEditedText", selectedNote?.text);
  }, [selectedNote]);

  const { mutate: updateNote } = useUpdateNote(
    electionRoundId,
    pollingStationId,
    formId,
    `Note_${electionRoundId}_${pollingStationId}_${formId}_${questionId}`,
  );

  const onSubmit = (formData: any) => {
    const updateNotePayload = {
      questionId,
      electionRoundId,
      pollingStationId,
      formId,
      id: selectedNote ? selectedNote?.id : "",
      text: formData.noteEditedText,
    };
    // update the note
    updateNote(updateNotePayload);
    // close dialog
    setSelectedNote(null);
  };

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

  return (
    <Sheet
      modal
      native
      open={!!selectedNote}
      zIndex={100_000}
      onOpenChange={(open: boolean) => {
        if (!open) {
          setSelectedNote(null);
        }
      }}
      snapPointsMode="fit"
      moveOnKeyboardChange={true}
      dismissOnSnapToBottom={true}
    >
      <Sheet.Overlay />
      <Sheet.Frame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
        paddingBottom="$xl"
        marginBottom={insets.bottom}
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <YStack marginHorizontal={12} gap="$md">
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

          <Controller
            key={selectedNote?.id + "_edit_note"}
            name={"noteEditedText"}
            control={control}
            render={({ field: { value, onChange } }) => {
              return (
                <YStack height={150}>
                  <Input type="textarea" value={value} onChangeText={onChange} height={150} />
                </YStack>
              );
            }}
          />

          <XStack gap="$md">
            <Button preset="chromeless" onPress={() => setSelectedNote(null)}>
              Cancel
            </Button>

            <Button flex={1} onPress={handleSubmit(onSubmit)}>
              Save
            </Button>
          </XStack>
        </YStack>
      </Sheet.Frame>
    </Sheet>
  );
};

export default EditNoteSheet;
