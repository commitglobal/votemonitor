import React from "react";
import { YStack } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";

export interface Report {
  key: string;
  title?: string;
  description?: string;
  attachments?: number;
}

export interface ReportCardProps extends CardProps {
  // report: Report;
  key: string;
  title?: string;
  description?: string;
  attachments?: number;
  onPress?: () => void;
}

const ReportCard = (props: ReportCardProps): JSX.Element => {
  const { title, description, onPress } = props;

  return (
    <Card onPress={onPress}>
      <YStack gap={16}>
        <Typography fontSize={16} color="$gray9" fontWeight="500">
          {title}
        </Typography>
        <Typography>{description}</Typography>
        <CardFooter text={"Att"} />
      </YStack>
    </Card>
  );
};

export default ReportCard;
