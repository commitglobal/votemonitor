import React from "react";
import { YStack } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { useTranslation } from "react-i18next";
export interface ReportCardProps extends CardProps {
  title: string;
  numberOfAttachments: number;
  description: string;
  onPress?: () => void;
}

const ReportCard = ({
  title,
  description,
  numberOfAttachments,
  onPress,
}: ReportCardProps): JSX.Element => {
  const { t } = useTranslation("report");

  return (
    <Card onPress={onPress} marginTop="$xxs">
      <YStack gap="$md">
        <Typography preset="body2">{title}</Typography>
        <Typography>{description}</Typography>
        <CardFooter
          text={
            numberOfAttachments === 0
              ? t("no_files")
              : numberOfAttachments === 1
                ? t("1_file", { value: numberOfAttachments })
                : t("files", { value: numberOfAttachments })
          }
        />
      </YStack>
    </Card>
  );
};

export default ReportCard;
