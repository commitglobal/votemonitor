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
  const { t } = useTranslation(["observation", "common"]);

  return (
    <>
      <XStack justifyContent="space-between">
        <Typography preset="heading" fontWeight="500">
          {t("polling_stations_information.polling_station_form.number_of_questions", {
            value: `${nrOfAnswers}/${nrOfQuestions}`,
          })}
        </Typography>
        <Badge status={nrOfAnswers === nrOfQuestions ? Status.COMPLETED : Status.IN_PROGRESS}>
          {nrOfAnswers === nrOfQuestions
            ? t("status.completed", { ns: "common" })
            : t("status.in_progress", { ns: "common" })}
        </Badge>
      </XStack>
    </>
  );
};

export default PollingStationInfo;
