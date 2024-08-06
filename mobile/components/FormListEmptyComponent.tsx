import React from "react";
import { XStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";

const FormListEmptyComponent = () => {
  const { t } = useTranslation("observation");
  return (
    <XStack gap="$md" alignItems="center">
      <Icon icon="form" />
      <Typography color="$gray5" flex={0.95}>
        {t("forms.list.empty")}
      </Typography>
    </XStack>
  );
};

export default FormListEmptyComponent;
