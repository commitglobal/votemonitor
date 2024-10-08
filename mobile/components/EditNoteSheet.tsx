import React, { useEffect, useMemo, useState } from "react";
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
import { Animated, BackHandler, Platform } from "react-native";
import useAnimatedKeyboardPadding from "../hooks/useAnimatedKeyboardPadding";

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
  const [deletingNote, setDeletingNote] = useState(false);
  const { t } = useTranslation(["polling_station_form_wizard", "common"]);

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

  // on Android back button press, if the sheet is open, we first close the sheet
  // and on the 2nd press we will navigate back
  useEffect(() => {
    if (Platform.OS !== "android") {
      return;
    }

    const onBackPress = () => {
      if (selectedNote) {
        setSelectedNote(null);
        return true;
      } else {
        // navigate back
        return false;
      }
    };
    const subscription = BackHandler.addEventListener("hardwareBackPress", onBackPress);
    return () => subscription.remove();
  }, [selectedNote, setSelectedNote]);

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
    }
    // close dialog
    setSelectedNote(null);
    setDeletingNote(false);
  };

  const AnimatedSheetFrame = useMemo(() => Animated.createAnimatedComponent(Sheet.Frame), []);
  const paddingBottom = useAnimatedKeyboardPadding(16);

  return (
    <Sheet
      modal
      open={!!selectedNote}
      zIndex={100_001}
      onOpenChange={(open: boolean) => {
        if (!open) {
          setSelectedNote(null);
        }
      }}
      snapPointsMode="fit"
      dismissOnSnapToBottom={true}
      disableDrag
    >
      <Sheet.Overlay />
      <AnimatedSheetFrame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
        paddingBottom={paddingBottom}
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />

        <YStack marginHorizontal={12} gap="$md">
          {deletingNote ? (
            <>
              <YStack gap="$lg">
                <Typography preset="heading">{t("notes.delete.title")}</Typography>
                <Typography preset="body1" color="$gray6">
                  {t("notes.delete.description")}
                </Typography>
                <XStack gap="$sm" justifyContent="center">
                  <Button
                    preset="chromeless"
                    textStyle={{ color: "black" }}
                    onPress={() => setDeletingNote(false)}
                  >
                    {t("notes.delete.actions.cancel")}
                  </Button>

                  <Button
                    backgroundColor="$red10"
                    flex={1}
                    onPress={() => {
                      onDelete();
                    }}
                  >
                    {t("notes.delete.actions.delete")}
                  </Button>
                </XStack>
              </YStack>
            </>
          ) : (
            <>
              <XStack justifyContent="space-between" alignItems="center">
                <Typography preset="heading">{t("notes.edit.heading")}</Typography>
                <Typography
                  preset="body2"
                  color="$red10"
                  paddingVertical="$xxxs"
                  paddingLeft="$xs"
                  pressStyle={{ opacity: 0.5 }}
                  onPress={() => setDeletingNote(true)}
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
                  required: { value: true, message: t("notes.add.form.input.required") },
                  validate: (value) =>
                    (value && value.trim().length > 0) || t("notes.add.form.input.required"),
                }}
                render={({ field: { value, onChange } }) => {
                  return (
                    <YStack height={150}>
                      <Input
                        type="textarea"
                        value={value}
                        onChangeText={onChange}
                        height={150}
                        placeholder={t("notes.add.form.input.placeholder")}
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
            </>
          )}
        </YStack>
      </AnimatedSheetFrame>
    </Sheet>
  );
};

export default EditNoteSheet;
