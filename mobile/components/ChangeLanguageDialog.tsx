import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import RadioFormInput from "./FormInputs/RadioFormInput";
import { Typography } from "./Typography";
import { Controller, useForm } from "react-hook-form";
import { Dialog } from "./Dialog";
import Button from "./Button";

interface ChangeLanguageDialogProps {
  formId: string;
  defaultLanguage?: string;
  languages: string[];
  onSelectLanguage: (formId: string, language: string) => void;
  onCancel: () => void;
}

const ChangeLanguageDialog = ({
  formId,
  languages,
  onSelectLanguage,
  onCancel,
}: ChangeLanguageDialogProps) => {
  const { t } = useTranslation(["form_overview", "languages"]);

  const languageMapping: { [key: string]: string } = {
    RO: t("ro", { ns: "languages" }),
    EN: t("en", { ns: "languages" }),
    PL: t("pl", { ns: "languages" }),
    BG: t("bg", { ns: "languages" }),
  };

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});

  const transformedLanguages = languages.map((language) => ({
    id: language,
    value: language,
    // TODO: decide if we add the name to the label as well
    label: languageMapping[language] || language,
  }));

  const onSubmit = (formValues: Record<string, string>) => {
    onSelectLanguage(formId, formValues[formId]);
  };

  return (
    <Controller
      key={formId}
      name={formId}
      control={control}
      rules={{
        required: { value: true, message: t("language_modal.error") },
      }}
      render={({ field: { onChange, value } }) => (
        <Dialog
          open
          header={<Typography preset="heading">{t("language_modal.header")}</Typography>}
          content={
            <YStack>
              <Typography preset="body1" marginBottom="$lg">
                {t("language_modal.helper")}
              </Typography>
              <RadioFormInput
                options={transformedLanguages}
                value={value}
                onValueChange={onChange}
              />
              {errors[formId] && (
                <Typography marginTop="$sm" style={{ color: "red" }}>
                  {`${errors[formId].message}`}
                </Typography>
              )}
            </YStack>
          }
          footer={
            <XStack gap="$md">
              <Button preset="chromeless" onPress={onCancel}>
                {t("language_modal.actions.cancel")}
              </Button>
              <Button onPress={handleSubmit(onSubmit)} flex={1}>
                {t("language_modal.actions.save")}
              </Button>
            </XStack>
          }
        />
      )}
    />
  );
};

export default ChangeLanguageDialog;
