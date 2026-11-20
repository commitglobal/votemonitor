import { useState } from "react";
import { useTranslation } from "react-i18next";
import { YStack, YStackProps } from "tamagui";
import { Typography } from "./Typography";
import { FormAPIModel } from "../services/definitions.api";

/** The form description in the given language, else the form's default one; empty when it has none. */
export const getFormDescription = (form: FormAPIModel | undefined, language: string): string => {
  if (!form?.description) return "";
  return (
    form.description[language] ||
    form.description[form.defaultLanguage] ||
    Object.values(form.description).find((text) => !!text?.trim()) ||
    ""
  ).trim();
};

const COLLAPSED_LINES = 3;
// about what 3 lines hold on a phone: a longer description is cut, with "Show more"
const LONG_DESCRIPTION = 160;

interface FormDescriptionProps extends YStackProps {
  form: FormAPIModel | undefined;
  language: string;
  /** shown in full (e.g. in the form menu's sheet) instead of cut after 3 lines */
  expanded?: boolean;
}

/**
 * The form's description, where a form starts. A long one is cut after 3 lines with "Show more",
 * so it doesn't push the form's first action off the screen. Nothing is shown without one.
 */
const FormDescription = ({ form, language, expanded = false, ...rest }: FormDescriptionProps) => {
  const { t } = useTranslation("common");
  const [isExpanded, setIsExpanded] = useState(expanded);
  const description = getFormDescription(form, language);
  const isLong = description.length > LONG_DESCRIPTION;

  if (!description) return null;

  return (
    <YStack gap="$xxs" {...rest}>
      <Typography
        preset="body1"
        color="$gray6"
        numberOfLines={isExpanded || !isLong ? undefined : COLLAPSED_LINES}
      >
        {description}
      </Typography>
      {isLong && !expanded && (
        <Typography
          preset="body2"
          color="$purple5"
          accessibilityRole="button"
          onPress={() => setIsExpanded((current) => !current)}
          hitSlop={8}
          alignSelf="flex-start"
        >
          {isExpanded ? t("form_description.show_less") : t("form_description.show_more")}
        </Typography>
      )}
    </YStack>
  );
};

export default FormDescription;
