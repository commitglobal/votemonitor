import React from "react";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import Card from "./Card";
import CardFooter from "./CardFooter";
import { Typography } from "./Typography";

interface IncidentReportCardProps {
  formName: string;
  location: string;
  numberOfAnswers: number;
  numberOfAttachments: number;
  onPress?: () => void;
}

const IncidentReportCard = ({
  formName,
  location,
  numberOfAnswers,
  numberOfAttachments,
  onPress,
}: IncidentReportCardProps): React.ReactElement => {
  const { t } = useTranslation(["incident_report", "quick_report"]);

  return (
    <Card onPress={onPress} marginTop="$xxs">
      <YStack gap="$md">
        <Typography preset="body2">{formName}</Typography>
        {location ? <Typography>{location}</Typography> : false}
        <Typography>{t("list.answers", { count: numberOfAnswers })}</Typography>
        <CardFooter
          text={
            numberOfAttachments === 0
              ? t("list.no_files", { ns: "quick_report" })
              : numberOfAttachments === 1
                ? t("list.attachment_one", { ns: "quick_report", count: numberOfAttachments })
                : t("list.attachment_other", { ns: "quick_report", count: numberOfAttachments })
          }
        />
      </YStack>
    </Card>
  );
};

export default IncidentReportCard;
