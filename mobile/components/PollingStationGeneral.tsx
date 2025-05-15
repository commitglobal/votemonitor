import { router } from "expo-router";
import { useCallback, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import { useUserData } from "../contexts/user/UserContext.provider";
import {
  PollingStationInformationAPIResponse,
  PollingStationInformationFormAPIResponse,
} from "../services/definitions.api";
import Card from "./Card";
import CardFooter from "./CardFooter";
import { Icon } from "./Icon";
import { openMaps } from "./MapOpener";
import PollingStationInfo from "./PollingStationInfo";
import PollingStationInfoDefault from "./PollingStationInfoDefault";
import { PSITime } from "./PSITime";
import { Typography } from "./Typography";

interface PollingStationGeneralProps {
  psiData: PollingStationInformationAPIResponse | null | undefined;
  psiFormQuestions: PollingStationInformationFormAPIResponse;
}

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  psiData: psi,
  psiFormQuestions,
}) => {
  const [hasCoordinates, setHasCoordinates] = useState(false);
  const { t } = useTranslation("observation");
  const { selectedPollingStation } = useUserData();

  useEffect(() => {
    if (!selectedPollingStation) {
      setHasCoordinates(false);
    }

    if (selectedPollingStation?.latitude && selectedPollingStation.longitude) {
      setHasCoordinates(true);
    }
  }, [selectedPollingStation]);

  const navigateToPollingStation = useCallback(() => {
    if (!hasCoordinates) {
      return;
    }
    openMaps(selectedPollingStation);
  }, [selectedPollingStation, hasCoordinates]);

  return (
    <YStack gap="$xxs">
      <XStack justifyContent="space-between" onPress={() => navigateToPollingStation()}>
        <Typography preset="body2" fontWeight="700" color="$gray7">
          {t("polling_stations_information.heading")}
        </Typography>

        {hasCoordinates ? <Icon icon="map" color="$purple5" /> : null}
      </XStack>

      <PSITime psiData={psi} />

      {/* only display the PSI card for polling stations that have a configured PSI form */}
      {psiFormQuestions && psiFormQuestions.questions && psiFormQuestions.questions.length > 0 && (
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
      )}
    </YStack>
  );
};
