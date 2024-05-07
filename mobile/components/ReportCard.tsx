import React from "react";
import { YStack } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { QuickReportsAPIResponse } from "../services/definitions.api";

export interface ReportCardProps extends CardProps {
  key: string;
  report: QuickReportsAPIResponse;
  onPress?: () => void;
}

const ReportCard = (props: ReportCardProps): JSX.Element => {
  const { report, onPress } = props;

  let attachmentText = `${report.attachments.length} attachment files`;

  if (report.attachments.length === 0) {
    attachmentText = "No attachments files";
  }

  return (
    <Card onPress={onPress}>
      <YStack gap={16}>
        <Typography fontSize={16} color="$gray9" fontWeight="500">
          {report.title}
        </Typography>
        <Typography>{report.description}</Typography>
        <CardFooter text={attachmentText} />
      </YStack>
    </Card>
  );
};

export default ReportCard;
