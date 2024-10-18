import { router } from "expo-router";
import { YStack } from "tamagui";
import PollingStationInfo from "./PollingStationInfo";
import PollingStationInfoDefault from "./PollingStationInfoDefault";
import {
  PollingStationInformationAPIResponse,
  PollingStationInformationFormAPIResponse,
} from "../services/definitions.api";
import CardFooter from "./CardFooter";
import Card from "./Card";
import { useTranslation } from "react-i18next";
import { Typography } from "./Typography";
import { PSITime } from "./PSITime";

interface PollingStationGeneralProps {
  psiData: PollingStationInformationAPIResponse | undefined | null;
  psiFormQuestions: PollingStationInformationFormAPIResponse | undefined;
}

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  psiData: psi,
  psiFormQuestions,
}) => {
  const { t } = useTranslation("observation");

  return (
    <YStack gap="$xxs">
      <Typography preset="body2" fontWeight="700" color="$gray7">
        {t("polling_stations_information.heading")}
      </Typography>

      <PSITime psiData={psi} />

      <Card gap="$md" onPress={router.push.bind(null, "/polling-station-questionnaire")}>
        {!psi?.answers?.length && !psi?.isCompleted ? (
          <PollingStationInfoDefault
            onPress={router.push.bind(null, "/polling-station-questionnaire")}
          />
        ) : (
          <PollingStationInfo
            nrOfAnswers={psi?.answers?.length}
            nrOfQuestions={psiFormQuestions?.questions?.length}
            isMarkedAsCompleted={psi?.isCompleted}
          />
        )}
        <CardFooter
          text={t("polling_stations_information.polling_station_form.form_details_button_label")}
        ></CardFooter>
      </Card>
    </YStack>
  );
};
