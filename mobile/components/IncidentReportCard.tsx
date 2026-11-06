import React from "react";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import Badge, { Status } from "./Badge";
import Card from "./Card";
import { Icon } from "./Icon";
import { useDateTimeFormat } from "../contexts/date-time-format/DateTimeFormatContext.provider";
import CardFooter from "./CardFooter";
import { Typography } from "./Typography";

interface IncidentReportCardProps {
  formName: string;
  location: string;
  numberOfAnswers: number;
  numberOfAttachments: number;
  // a draft (not completed) can still be continued
  isCompleted: boolean;
  // when it was sent (or, for a draft, last saved)
  timestamp?: string;
  onPress?: () => void;
}

const IncidentReportCard = ({
  formName,
  location,
  numberOfAnswers,
  numberOfAttachments,
  isCompleted,
  timestamp,
  onPress,
}: IncidentReportCardProps): React.ReactElement => {
  const { t } = useTranslation(["incident_report", "quick_report", "common"]);
  const { formatDate, formatTime } = useDateTimeFormat();
  const date = timestamp ? new Date(timestamp) : undefined;

  return (
    <Card onPress={onPress} marginTop="$xxs">
      <YStack gap="$md">
        <XStack justifyContent="space-between" alignItems="flex-start" gap="$xs">
          <Typography preset="body2" flexShrink={1}>
            {formName}
          </Typography>
          <Badge status={isCompleted ? Status.COMPLETED : Status.IN_PROGRESS}>
            {isCompleted
              ? t("status.completed", { ns: "common" })
              : t("status.in_progress", { ns: "common" })}
          </Badge>
        </XStack>
        {date ? (
          <XStack alignItems="center" gap="$xxs">
            <Icon icon="clock" color="$gray5" size={16} />
            <Typography color="$gray5">{`${formatDate(date)} ${formatTime(date)}`}</Typography>
          </XStack>
        ) : (
          false
        )}
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
