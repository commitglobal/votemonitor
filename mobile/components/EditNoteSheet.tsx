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
import { useTranslation } from "react-i18next";
import { Keyboard, Platform } from "react-native";
import { useKeyboardVisible } from "@tamagui/use-keyboard-visible";

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
  const { t } = useTranslation(["polling_station_form_wizard", "common"]);
  const insets = useSafeAreaInsets();
  const keyboardIsVisible = useKeyboardVisible();

  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({
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
      open={!!selectedNote}
      zIndex={100_000}
      onOpenChange={(open: boolean) => {
        if (!open) {
          setSelectedNote(null);
        }
      }}
      snapPointsMode="fit"
      // seems that this behaviour is handled differently and the sheet will move with keyboard even if this props is set to false on android
      moveOnKeyboardChange={Platform.OS === "android"}
      dismissOnSnapToBottom={true}
      disableDrag
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
        <YStack
          marginHorizontal={12}
          gap="$md"
          paddingBottom={
            // add padding if keyboard is visible
            Platform.OS === "ios" && keyboardIsVisible && Keyboard.metrics()?.height
              ? // @ts-ignore: it will not be undefined because we're checking above
                Keyboard.metrics()?.height - insets.bottom
              : 0
          }
        >
          <XStack justifyContent="space-between" alignItems="center">
            <Typography preset="heading">{t("notes.edit.heading")}</Typography>
            <Typography
              preset="body2"
              color="$red10"
              paddingVertical="$xxxs"
              paddingLeft="$xs"
              pressStyle={{ opacity: 0.5 }}
              onPress={onDelete}
            >
              {t("notes.edit.delete")}
            </Typography>
          </XStack>

          <Controller
            key={selectedNote?.id + "_edit_note"}
            name={"noteEditedText"}
            control={control}
            rules={{
              maxLength: {
                value: 10000,
                message: t("notes.add.form.input.max"),
              },
            }}
            render={({ field: { value, onChange } }) => {
              return (
                <YStack height={150}>
                  <Input
                    type="textarea"
                    placeholder={t("notes.add.form.input.placeholder")}
                    value={value}
                    onChangeText={onChange}
                    height={150}
                  />
                </YStack>
              );
            }}
          />
          {errors.noteEditedText && (
            <Typography color="$red12">{errors.noteEditedText.message}</Typography>
          )}
          <XStack gap="$md">
            <Button preset="chromeless" onPress={() => setSelectedNote(null)}>
              {t("cancel", { ns: "common" })}
            </Button>

            <Button flex={1} onPress={handleSubmit(onSubmit)}>
              {t("save", { ns: "common" })}
            </Button>
          </XStack>
        </YStack>
      </Sheet.Frame>
    </Sheet>
  );
};

export default EditNoteSheet;
