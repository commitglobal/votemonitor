import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
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
        <Typography preset="heading" fontWeight="500" maxWidth="55%">
          {t("polling_stations_information.polling_station_form.number_of_questions", {
            value: `${nrOfAnswers}/${nrOfQuestions}`,
          })}
        </Typography>
      </XStack>
    </>
  );
};

export default PollingStationInfo;
