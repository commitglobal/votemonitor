import React, { Dispatch, SetStateAction } from "react";
import { ScrollView, XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import Button from "./Button";
import { Controller, useForm } from "react-hook-form";
import Input from "./Inputs/Input";
import { useAddNoteMutation } from "../services/mutations/add-note.mutation";
import { Keyboard } from "react-native";
import * as Crypto from "expo-crypto";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(["polling_station_form_wizard", "common"]);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
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

  const onSubmitNote = (note: { noteText: string }) => {
    const notePayload = {
      id: Crypto.randomUUID(),
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
    <ScrollView
      marginHorizontal={12}
      contentContainerStyle={{ gap: 16 }}
      keyboardShouldPersistTaps="handled"
      bounces={false}
    >
      <Typography preset="heading">{t("notes.add.heading")}</Typography>

      <Controller
        key={questionId + "_note"}
        name={"noteText"}
        control={control}
        rules={{
          required: { value: true, message: t("notes.add.form.input.required") },
          maxLength: {
            value: 10000,
            message: t("notes.add.form.input.max"),
          },
          // check if the user actually entered something other than whitespaces
          validate: (value) => value.trim().length > 0 || t("notes.add.form.input.required"),
        }}
        render={({ field: { value: noteValue, onChange: onNoteChange } }) => {
          return (
            <YStack height={150}>
              <Input
                type="textarea"
                placeholder={t("notes.add.form.input.placeholder")}
                value={noteValue}
                height={150}
                onChangeText={onNoteChange}
              />
            </YStack>
          );
        }}
      />
      {errors.noteText && <Typography color="$red12">{errors.noteText.message}</Typography>}
      <XStack gap="$md">
        <Button preset="chromeless" onPress={() => setAddingNote(false)}>
          {t("cancel", { ns: "common" })}
        </Button>
        <Button flex={1} onPress={handleSubmit(onSubmitNote)}>
          {t("save", { ns: "common" })}
        </Button>
      </XStack>
    </ScrollView>
  );
};

export default AddNoteSheetContent;
