import React from "react";
import { useTranslation } from "react-i18next";
import { View, styled } from "tamagui";
import { ReportType } from "../services/definitions.api";
import Badge, { Status } from "./Badge";
import Card, { CardProps } from "./Card";
import CardFooter from "./CardFooter";
import { Typography } from "./Typography";

export interface ReportCardProps extends CardProps {
  title: string;
  numberOfAttachments: number;
  description: string;
  reportType: ReportType;
  onPress?: () => void;
}

const ReportCard = ({
  title,
  description,
  numberOfAttachments,
  reportType,
  onPress,
}: ReportCardProps): JSX.Element => {
  const { t } = useTranslation("quick_report");

  const CardHeader = styled(View, {
    name: "CardHeader",
    justifyContent: "space-between",
    flexDirection: "row",
    alignItems: "center",
    marginBottom: "$xxs",
  });

  return (
    <Card width="100%" onPress={onPress}>
      <CardHeader>
        <Typography preset="body1" color="$gray9" fontWeight="700" maxWidth="55%" numberOfLines={2}>
          {title}
        </Typography>

        <Badge status={Status.NOT_STARTED} maxWidth="45%" textStyle={{ textAlign: "center" }}>
          {reportType === ReportType.QuickReport ? "Quick report" : "Incident report"}
        </Badge>
      </CardHeader>

      <Typography>{description}</Typography>

      <CardFooter
        text={
          numberOfAttachments === 0
            ? t("list.no_files")
            : numberOfAttachments === 1
              ? t("list.attachment_one", { count: numberOfAttachments })
              : t("list.attachment_other", { count: numberOfAttachments })
        }
      />
    </Card>
  );
};

export default ReportCard;
