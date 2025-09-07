import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import RadioFormInput from "./FormInputs/RadioFormInput";
import { Typography } from "./Typography";

const SelectFormLanguageDialogContent = ({
  languages,
  error,
  value,
  onChange,
}: {
  languages: string[];
  error: FieldError | Merge<FieldError, FieldErrorsImpl<any>> | undefined;
  value: string;
  onChange: (...event: any[]) => void;
}) => {
  const { t } = useTranslation(["observation", "languages"]);

  const languageMapping: { [key: string]: string } = {
    RO: t("ro", { ns: "languages" }),
    EN: t("en", { ns: "languages" }),
    PL: t("pl", { ns: "languages" }),
    BG: t("bg", { ns: "languages" }),
    KA: t("ka", { ns: "languages" }),
    HY: t("hy", { ns: "languages" }),
    RU: t("ru", { ns: "languages" }),
    AZ: t("az", { ns: "languages" }),
    ES: t("es", { ns: "languages" }),
  };

  const transformedLanguages = languages.map((language) => ({
    id: language,
    value: language,
    // TODO: decide if we add the name to the label as well
    label: languageMapping[language] || language,
  }));

  return (
    <YStack>
      <Typography preset="body1" marginBottom="$lg">
        {t("forms.select_language_modal.helper")}
      </Typography>
      <RadioFormInput options={transformedLanguages} value={value} onValueChange={onChange} />
      {error && (
        <Typography marginTop="$sm" style={{ color: "red" }}>
          {`${error.message}`}
        </Typography>
      )}
    </YStack>
  );
};

export default SelectFormLanguageDialogContent;
