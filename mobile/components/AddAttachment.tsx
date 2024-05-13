import React from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";

const AddAttachment = ({ ...rest }: XStackProps) => {
  const { t } = useTranslation("question_page");
  return (
    <XStack pressStyle={{ opacity: 0.5 }} {...rest} alignSelf="flex-start">
      <Icon icon="attachment" />
      <Typography color="$purple5" marginLeft="$xs">
        {t("actions.add_media")}
      </Typography>
    </XStack>
  );
};

export default AddAttachment;
