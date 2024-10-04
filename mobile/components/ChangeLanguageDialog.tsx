import { useTranslation } from "react-i18next";
import { ScrollView, XStack, YStack } from "tamagui";
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
  const { t } = useTranslation(["form_overview", "languages", "observation"]);

  const languageMapping: { [key: string]: string } = {
    RO: t("ro", { ns: "languages" }),
    EN: t("en", { ns: "languages" }),
    PL: t("pl", { ns: "languages" }),
    BG: t("bg", { ns: "languages" }),
    KA: t("ka", { ns: "languages" }),
    HY: t("hy", { ns: "languages" }),
    RU: t("ru", { ns: "languages" }),
    AZ: t("az", { ns: "languages" }),
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
        required: {
          value: true,
          message: t("forms.select_language_modal.error", { ns: "observation" }),
        },
      }}
      render={({ field: { onChange, value } }) => (
        <Dialog
          open
          header={
            <Typography preset="heading">
              {t("forms.select_language_modal.header", { ns: "observation" })}
            </Typography>
          }
          content={
            <ScrollView bounces={false} showsVerticalScrollIndicator={false}>
              <YStack>
                <Typography preset="body1" marginBottom="$lg">
                  {t("forms.select_language_modal.helper", { ns: "observation" })}
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
            </ScrollView>
          }
          footer={
            <XStack gap="$md">
              <Button preset="chromeless" onPress={onCancel}>
                {t("cancel", { ns: "common" })}
              </Button>
              <Button onPress={handleSubmit(onSubmit)} flex={1}>
                {t("save", { ns: "common" })}
              </Button>
            </XStack>
          }
        />
      )}
    />
  );
};

export default ChangeLanguageDialog;
