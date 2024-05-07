import React from "react";
import { View, styled } from "tamagui";
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
      <Typography>{report.title}</Typography>
      <Typography>{report.description}</Typography>
      <Typography>{report.attachments}</Typography>
    </Card>
  );
};

export default ReportCard;
