import React from "react";
import { YStack } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
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
}: ReportCardProps): JSX.Element => (
  <Card onPress={onPress} marginTop="$xxs">
    <YStack gap="$md">
      <Typography preset="body2">{title}</Typography>
      <Typography>{description}</Typography>
      <CardFooter
        text={
          numberOfAttachments === 0
            ? "No attachments files"
            : `${numberOfAttachments} attachment ${numberOfAttachments === 1 ? "file" : "files"}`
        }
      />
    </YStack>
  </Card>
);

export default ReportCard;
