import { router } from "expo-router";
import { XStack, YStack } from "tamagui";
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
import { Icon } from "./Icon";

interface PollingStationGeneralProps {
  psiData: PollingStationInformationAPIResponse | null | undefined;
  psiFormQuestions: PollingStationInformationFormAPIResponse;
}

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  psiData: psi,
  psiFormQuestions,
}) => {
  const { t } = useTranslation("observation");

  // PSI form not configured
  if (!psiFormQuestions || !psiFormQuestions.questions) {
    return (
      <YStack gap="$xxs">
        <Card>
          <XStack gap="$md" alignItems="center">
            <Icon icon="warning" color="$purple5" />
            <Typography preset="body2" color="$purple5" flex={1}>
              {t("no_form")}
            </Typography>
          </XStack>
        </Card>
        <Typography preset="helper" color="$gray5" fontWeight="400" flex={1}>
          {t("refresh_page")}
        </Typography>
      </YStack>
    );
  }

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
