import { router } from "expo-router";
import { XStack } from "tamagui";
import PollingStationInfo from "./PollingStationInfo";
import PollingStationInfoDefault from "./PollingStationInfoDefault";
import TimeSelect from "./TimeSelect";
import {
  PollingStationInformationAPIResponse,
  PollingStationInformationFormAPIResponse,
} from "../services/definitions.api";
import CardFooter from "./CardFooter";
import Card from "./Card";
import { useMutatePollingStationGeneralData } from "../services/mutations/psi-general.mutation";
import { ApiFormAnswer } from "../services/interfaces/answer.type";
import { useTranslation } from "react-i18next";
import Toast from "react-native-toast-message";

interface PollingStationGeneralProps {
  electionRoundId: string;
  pollingStationId: string;
  psiData: PollingStationInformationAPIResponse | null | undefined;
  psiFormQuestions: PollingStationInformationFormAPIResponse;
}

// TODO: move and reuse for mutation fn
type PollingStationInformationVM = {
  arrivalTime: string;
  departureTime: string;
  answers: ApiFormAnswer[];
};

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  electionRoundId,
  pollingStationId,
  psiData: psi,
  psiFormQuestions,
}) => {
  const { t } = useTranslation("observation");

  const { mutate } = useMutatePollingStationGeneralData({
    electionRoundId,
    pollingStationId,
    scopeId: `PS_General_${electionRoundId}_${pollingStationId}_dates`,
  });

  const _updateArrivalDepartureTime = (
    payload: Partial<Pick<PollingStationInformationVM, "arrivalTime" | "departureTime">>,
  ) => {
    mutate({ electionRoundId, pollingStationId, ...payload });
  };

  const onUpdateArrivalTime = (arrivalTime: Date) => {
    console.log("arrivalTime", arrivalTime);
    if (psi?.departureTime && arrivalTime.getTime() > new Date(psi.departureTime).getTime()) {
      return Toast.show({
        type: "error",
        text2: t("polling_stations_information.time_select.error.earlier_arrival"),
        visibilityTime: 5000,
      });
    }

    if (arrivalTime)
      mutate({ electionRoundId, pollingStationId, arrivalTime: arrivalTime.toISOString() });
  };

  const onUpdateDepartureTime = (departureTime: Date) => {
    // if we're trying to set a departure time before having set the arrival time -> close picker and display error toast
    if (!psi?.arrivalTime) {
      return Toast.show({
        type: "error",
        text2: t("polling_stations_information.time_select.error.arrival_first"),
        visibilityTime: 5000,
      });
    } else if (departureTime.getTime() < new Date(psi.arrivalTime).getTime()) {
      return Toast.show({
        type: "error",
        text2: t("polling_stations_information.time_select.error.later_departure"),
        visibilityTime: 5000,
      });
    }

    if (departureTime)
      mutate({ electionRoundId, pollingStationId, departureTime: departureTime.toISOString() });
  };

  return (
    <>
      <XStack gap="$xxs">
        <Card flex={0.5} paddingVertical="$xs">
          <TimeSelect
            time={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
            maximumDate={psi?.departureTime ? new Date(psi?.departureTime) : undefined}
            textFooter={t("polling_stations_information.time_select.arrival_time")}
            setTime={onUpdateArrivalTime}
          />
        </Card>
        <Card flex={0.5} paddingVertical="$xs">
          <TimeSelect
            textFooter={t("polling_stations_information.time_select.departure_time")}
            time={psi?.departureTime ? new Date(psi?.departureTime) : undefined}
            minimumDate={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
            setTime={onUpdateDepartureTime}
          />
        </Card>
      </XStack>

      <Card
        gap="$md"
        onPress={router.push.bind(null, "/polling-station-questionnaire")}
        marginTop="$xxs"
      >
        {!psi?.answers?.length ? (
          <PollingStationInfoDefault
            onPress={router.push.bind(null, "/polling-station-questionnaire")}
          />
        ) : (
          <PollingStationInfo
            nrOfAnswers={psi?.answers?.length}
            nrOfQuestions={psiFormQuestions?.questions?.length}
          />
        )}
        <CardFooter
          text={t("polling_stations_information.polling_station_form.form_details_button_label")}
        ></CardFooter>
      </Card>
    </>
  );
};
