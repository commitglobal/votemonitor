import React from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { ScrollView, XStack, YStack } from "tamagui";
import Button from "./Button";
import Input from "./Inputs/Input";
import { Typography } from "./Typography";

interface NoteFormProps {
  heading: string;
  initialText?: string;
  onSave: (text: string) => void;
  onCancel: () => void;
}

const NoteForm = ({ heading, initialText = "", onSave, onCancel }: NoteFormProps) => {
  const { t } = useTranslation(["polling_station_form_wizard", "common"]);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: {
      noteText: initialText,
    },
  });

  const onSubmitNote = ({ noteText }: { noteText: string }) => {
    onSave(noteText);
    reset();
  };

  return (
    <ScrollView
      marginHorizontal={12}
      contentContainerStyle={{ gap: 16 }}
      keyboardShouldPersistTaps="handled"
      bounces={false}
    >
      <Typography preset="heading">{heading}</Typography>

      <Controller
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
        <Button preset="chromeless" onPress={onCancel}>
          {t("cancel", { ns: "common" })}
        </Button>
        <Button flex={1} onPress={handleSubmit(onSubmitNote)}>
          {t("save", { ns: "common" })}
        </Button>
      </XStack>
    </ScrollView>
  );
};

export default NoteForm;
