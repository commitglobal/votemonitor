import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import Badge, { Status } from "./Badge";
import { useTranslation } from "react-i18next";

interface PollingStationInfoProps {
  nrOfAnswers: number | undefined;
  nrOfQuestions: number | undefined;
}

const PollingStationInfo: React.FC<PollingStationInfoProps> = ({
  nrOfAnswers = 0,
  nrOfQuestions = 0,
}) => {
  const { t } = useTranslation("observations_polling_station");

  return (
    <>
      <XStack justifyContent="space-between">
        <Typography preset="heading" fontWeight="500">
          {t("polling_station_card.questions", {
            value: `${nrOfAnswers}/${nrOfQuestions}`,
          })}
        </Typography>
        <Badge status={nrOfAnswers === nrOfQuestions ? Status.COMPLETED : Status.IN_PROGRESS}>
          {nrOfAnswers === nrOfQuestions
            ? t("polling_station_card.badge_status.completed")
            : t("polling_station_card.badge_status.in_progress")}
        </Badge>
      </XStack>
    </>
  );
};

export default PollingStationInfo;
