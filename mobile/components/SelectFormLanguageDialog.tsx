import React from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { XStack } from "tamagui";
import Button from "./Button";
import { Dialog } from "./Dialog";
import SelectFormLanguageDialogContent from "./SelectFormLanguageDialogContent";
import { Typography } from "./Typography";

interface SelectFormLanguageDialogProps {
  languages: string[];
  onConfirm: (language: string) => void;
  onCancel: () => void;
}

// asks which language to fill in a form that is available in several languages
const SelectFormLanguageDialog = ({
  languages,
  onConfirm,
  onCancel,
}: SelectFormLanguageDialogProps) => {
  const { t } = useTranslation(["observation", "common"]);
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<{ language: string }>({ defaultValues: { language: "" } });

  return (
    <Controller
      name="language"
      control={control}
      rules={{ required: { value: true, message: t("forms.select_language_modal.error") } }}
      render={({ field: { onChange, value } }) => (
        <Dialog
          open
          header={
            <Typography preset="heading">{t("forms.select_language_modal.header")}</Typography>
          }
          content={
            <SelectFormLanguageDialogContent
              languages={languages}
              error={errors.language}
              value={value}
              onChange={onChange}
            />
          }
          footer={
            <XStack gap="$md">
              <Button preset="chromeless" onPress={onCancel}>
                {t("cancel", { ns: "common" })}
              </Button>
              <Button onPress={handleSubmit(({ language }) => onConfirm(language))} flex={1}>
                {t("save", { ns: "common" })}
              </Button>
            </XStack>
          }
        />
      )}
    />
  );
};

export default SelectFormLanguageDialog;
