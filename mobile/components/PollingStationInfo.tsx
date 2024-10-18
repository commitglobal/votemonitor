import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import Badge, { Status } from "./Badge";

interface PollingStationInfoProps {
  nrOfAnswers: number | undefined;
  nrOfQuestions: number | undefined;
  isMarkedAsCompleted: boolean;
}

const PollingStationInfo: React.FC<PollingStationInfoProps> = ({
  nrOfAnswers = 0,
  nrOfQuestions = 0,
  isMarkedAsCompleted = false,
}) => {
  const { t } = useTranslation(["observation", "common"]);

  return (
    <>
      <XStack justifyContent="space-between">
        <Typography preset="heading" fontWeight="500" maxWidth="55%">
          {t("polling_stations_information.polling_station_form.number_of_questions", {
            value: `${nrOfAnswers}/${nrOfQuestions}`,
          })}
        </Typography>
        <Badge
          status={
            isMarkedAsCompleted
              ? Status.MARKED_AS_COMPLETED
              : nrOfAnswers === nrOfQuestions
                ? Status.COMPLETED
                : Status.IN_PROGRESS
          }
          maxWidth="45%"
          textStyle={{ textAlign: "center" }}
        >
          {isMarkedAsCompleted
            ? t("status.marked_as_completed", { ns: "common" })
            : nrOfAnswers === nrOfQuestions
              ? t("status.completed", { ns: "common" })
              : t("status.in_progress", { ns: "common" })}
        </Badge>
      </XStack>
    </>
  );
};

export default PollingStationInfo;
