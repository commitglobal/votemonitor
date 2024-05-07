import React from "react";
import { View, YStack, styled } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";

export interface Report {
  title?: string;
  description?: string;
  attachments?: number;
}

export interface ReportCardProps extends CardProps {
  report: Report;
  onPress: () => void;
}

const ReportCard = (props: ReportCardProps): JSX.Element => {
  const { report, onPress, ...rest } = props;

  return (
    <Card>
      <YStack gap={16}>
        <Typography fontSize={16} color="$gray9" fontWeight="500">
          {report.title}
        </Typography>
        <Typography>{report.description}</Typography>
        <CardFooter text={"Att"} />
      </YStack>
    </Card>
  );
};

export default ReportCard;
